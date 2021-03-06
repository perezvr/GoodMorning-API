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
        public async Task<IActionResult> Get([FromQuery] ForecastRequestDto request)
        {
            ForecastResponseDto responseBody = null;

            try
            {
                _logger.LogInformation("processing new GetForecast request");

                var forecastKeyPrefix = "forecast";

                var forecastKey = string.Join("|", forecastKeyPrefix, request.Lat, request.Lon);

                //try
                //{
                //    var cachedObject = await _distributedCache.GetStringAsync(forecastKey);
                //    responseBody = JsonConvert.DeserializeObject<ForecastResponseDto>(cachedObject ?? string.Empty);
                //}
                //catch (Exception)
                //{
                //    //do nothing
                //}

                if (responseBody != null)
                    _logger.LogInformation("returned by Redis");
                else
                {
                    var client = new RestClient($"https://api.hgbrasil.com/weather?city_name={request.City_Name}&array_limit=7&key={_configuration.GetSection("HgBrasilApi:Key").Value}");
                    var forecastRequest = new RestRequest(Method.GET);

                    IRestResponse response = client.Execute(forecastRequest);

                    responseBody = JsonConvert.DeserializeObject<ForecastResponseDto>(response.Content);

                    var memoryCacheEntryOptions = new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(int.Parse(_configuration.GetSection("Redis:GetForecastTimeout").Value)),
                    };

                    //try
                    //{
                    //    await _distributedCache.SetStringAsync(forecastKey, response.Content, memoryCacheEntryOptions);
                    //}
                    //catch (Exception)
                    //{
                    //    //do nothing
                    //}

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
