using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Redis.Dto.Quotation;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Redis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FinanceController : ControllerBase
    {
        private readonly ILogger<FinanceController> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly IConfiguration _configuration;

        public FinanceController(
            ILogger<FinanceController> logger,
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
            QuotationResponse responseBody = null;

            try
            {
                _logger.LogInformation("processing new GetQuotation request");

                var quotationKey = "quotation";
                var cachedObject = await _distributedCache.GetStringAsync(quotationKey);

                responseBody = JsonConvert.DeserializeObject<QuotationResponse>(cachedObject ?? string.Empty);

                if (responseBody != null)
                    _logger.LogInformation("returned by Redis");
                else
                {
                    var client = new RestClient($"https://api.hgbrasil.com/finance?key={_configuration.GetSection("HgBrasilApi:Key").Value}");
                    var quotationRequest = new RestRequest(Method.GET);

                    IRestResponse response = client.Execute(quotationRequest);

                    responseBody = JsonConvert.DeserializeObject<QuotationResponse>(response.Content);

                    var memoryCacheEntryOptions = new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(int.Parse(_configuration.GetSection("Redis:GetQuotationTimeout").Value)),
                    };

                    await _distributedCache.SetStringAsync(quotationKey, response.Content, memoryCacheEntryOptions);

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

