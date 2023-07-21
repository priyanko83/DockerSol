using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AzureADAuthenticationUtilities;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GatewayApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly EnvironmentConfig _configuration;
        private readonly AzureAdOptions _azureAdOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TelemetryClient _telemetryClient;


        public ValuesController(IOptions<AzureAdOptions> azureOptions, IHttpContextAccessor httpContextAccessor, TelemetryClient telemetryClient)
        {
        
            _azureAdOptions = azureOptions.Value;
            _httpContextAccessor = httpContextAccessor;
            _telemetryClient = telemetryClient;
        }

        // GET api/values
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            //return new string[] { "Test 123456" };
            return new string[] {
                "GatewayApiiii --- GatewayApi machine name: " + Environment.MachineName + " --- " + DateTime.Now.ToString("yyyyMMddHHmmss"),
                "GatewayApi invoked Customer Api --- Result: ",
                await InvokeCustomerAPI()
            };
        }

        private async Task<string> InvokeCustomerAPI()
        {
            TableStorageWriter writer = new TableStorageWriter();
            try
            {
                _telemetryClient.TrackEvent("Gatewayapi.InvokeCustomerAPI", new Dictionary<string, string>() { { "Time Of Invocation" , DateTime.Now.ToString()  } });

                var customerapiurl = _azureAdOptions.CustomerApiBaseUrl + "/api/values";
                
                writer.Write("Customer Api url: " + customerapiurl);
                var result = await OnBehalfOfTokenGenerator.FetchToken(_httpContextAccessor, _azureAdOptions);


                var request = (HttpWebRequest)WebRequest.Create(customerapiurl);
                request.Method = "GET";
                request.Headers["Authorization"] = string.Format("Bearer {0}", result.AccessToken);
                string content = null;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();
                        }
                    }
                }
                return content;
            }
            catch (Exception ex)
            {
                writer.Write("InvokeCustomerAPI Error: " + ex.Message);
                return ex.Message;
            }
        }
    }

    public class EnvironmentConfig
    {
        public string CustomerApiUrl { get; set; }
    }
}