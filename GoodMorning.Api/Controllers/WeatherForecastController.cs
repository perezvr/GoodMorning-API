using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Redis.Dto.WeatherForecast;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Redis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly IConfiguration _configuration;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IDistributedCache distributedCache,
            IConfiguration configuration)
        {
            _logger = logger;
            _distributedCache = distributedCache;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ForecastRequest request)
        {
            ForecastResponse responseBody = null;

            try
            {
                _logger.LogInformation("processing new GetForecast request");

                var forecastKeyPrefix = "forecast";

                var forecastKey = string.Join("|", forecastKeyPrefix, request.Lat, request.Lon);
                var cachedObject = await _distributedCache.GetStringAsync(forecastKey);

                responseBody = JsonConvert.DeserializeObject<ForecastResponse>(cachedObject ?? string.Empty);

                if (responseBody != null)
                    _logger.LogInformation("returned by Redis");
                else
                {
                    var client = new RestClient($"https://api.hgbrasil.com/weather?woeid=455827");
                    var forecastRequest = new RestRequest(Method.GET);

                    IRestResponse response = client.Execute(forecastRequest);

                    responseBody = JsonConvert.DeserializeObject<ForecastResponse>(response.Content);

                    var memoryCacheEntryOptions = new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(int.Parse(_configuration.GetSection("Redis:GetForecastTimeout").Value)),
                    };

                    await _distributedCache.SetStringAsync(forecastKey, response.Content, memoryCacheEntryOptions);

                    _logger.LogInformation("returned by API");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            if (responseBody != null)
                return Ok(responseBody);
            else
                return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}
