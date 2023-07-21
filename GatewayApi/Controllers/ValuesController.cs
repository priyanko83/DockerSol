using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AzureADAuthenticationUtilities.GraphApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using AzureADAuthenticationUtilities;

namespace GatewayApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly EnvironmentConfig _configuration;
        private readonly AzureAdOptions _azureAdOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ValuesController(IOptions<EnvironmentConfig> configuration, IOptions<AzureAdOptions> azureOptions, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration.Value;
            _azureAdOptions = azureOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET api/values
        [Authorize]
        [HttpGet]        
        public async Task<IEnumerable<string>> Get()
        {            
            return new string[] {
                "GatewayApi --- GatewayApi machine name: " + Environment.MachineName + " --- " + DateTime.Now.ToString("yyyyMMddHHmmss") + " --- CustomerApiUrl from config.. : " + _configuration.CustomerApiUrl,
                "GatewayApi invoked Customer Api --- Result: ",
                await InvokeCustomerAPI()
            };
        }

        // GET api/values
        [Authorize]
        [HttpPost]
        public async Task<IEnumerable<string>> Post()
        {
            return new string[] {
                "GatewayApi --- GatewayApi machine name: " + Environment.MachineName + " --- " + DateTime.Now.ToString("yyyyMMddHHmmss") + " --- CustomerApiUrl from config.. : " + _configuration.CustomerApiUrl,
                "GatewayApi invoked Customer Api --- Result: ",
                await InvokeCustomerAPI()
            };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        public async Task<string> InvokeCustomerAPI()
        {
            TableStorageWriter writer = new TableStorageWriter();
            try
            {
                var customerapiurl = _configuration.CustomerApiUrl + "/api/values";
                if (string.IsNullOrEmpty(_configuration.CustomerApiUrl))
                {
                    customerapiurl = _azureAdOptions.CustomerApiBaseUrl + "/api/values";
                }
                
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
}
