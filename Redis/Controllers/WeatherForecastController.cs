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
        public async Task<IActionResult> Get()
        {
            Response responseBody = null;
            try
            {
                _logger.LogInformation("processing new GetForecast request");

                var forecastKey = "forecast";
                var timeout = int.Parse(_configuration.GetSection("Redis:GetForecastTimeout").Value);

                var cachedObject = await _distributedCache.GetStringAsync(forecastKey);

                responseBody = JsonConvert.DeserializeObject<Response>(cachedObject ?? string.Empty);

                if (responseBody != null)
                {
                    _logger.LogInformation("returned by Redis");
                }
                else
                {
                    var client = new RestClient("https://weatherbit-v1-mashape.p.rapidapi.com/forecast/daily?lat=-22.5252&lon=-44.1038");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("x-rapidapi-host", "weatherbit-v1-mashape.p.rapidapi.com");
                    request.AddHeader("x-rapidapi-key", "477cde4760msh954a62f20d6a2c8p1cb081jsna55c2b25b7a2");
                    IRestResponse response = client.Execute(request);

                    responseBody = JsonConvert.DeserializeObject<Response>(response.Content);

                    var memoryCacheEntryOptions = new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(timeout),
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
