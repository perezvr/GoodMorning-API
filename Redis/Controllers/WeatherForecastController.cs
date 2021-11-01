using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Redis.Dto;
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
        public async Task<IActionResult> Get([FromQuery] Request request)
        {
            Response responseBody = null;

            try
            {
                _logger.LogInformation("processing new GetForecast request");

                var forecastKeyPrefix = "forecast";

                var forecastKey = string.Join("|", forecastKeyPrefix, request.Lat, request.Lon);
                var cachedObject = await _distributedCache.GetStringAsync(forecastKey);

                responseBody = JsonConvert.DeserializeObject<Response>(cachedObject ?? string.Empty);

                if (responseBody != null)
                    _logger.LogInformation("returned by Redis");
                else
                {
                    var client = new RestClient($"https://weatherbit-v1-mashape.p.rapidapi.com/forecast/daily?lat={request.Lat}&lon={request.Lon}");
                    var forecastRequest = new RestRequest(Method.GET);
                    forecastRequest.AddHeader("x-rapidapi-host", "weatherbit-v1-mashape.p.rapidapi.com");
                    forecastRequest.AddHeader("x-rapidapi-key", "477cde4760msh954a62f20d6a2c8p1cb081jsna55c2b25b7a2");
                    IRestResponse response = client.Execute(forecastRequest);

                    responseBody = JsonConvert.DeserializeObject<Response>(response.Content);

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
