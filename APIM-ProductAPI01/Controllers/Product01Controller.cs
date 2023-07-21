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
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace ProductAPI01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class Product01Controller : ControllerBase
    {
        private readonly AzureAdOptions _azureAdOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TelemetryClient _telemetryClient;
        
        public Product01Controller(IOptions<AzureAdOptions> azureOptions, IHttpContextAccessor httpContextAccessor, TelemetryClient telemetryClient)
        {
            _azureAdOptions = azureOptions.Value;
            _httpContextAccessor = httpContextAccessor;
            _telemetryClient = telemetryClient;
        }

        [HttpGet]
        [Route("Ops01")]
        public IEnumerable<string> Operation01()
        {
            return new string[] { $"Product01 - Operation 01 - {DateTime.Now}" };
        }

        [HttpGet]
        [Route("Ops02")]
        public IEnumerable<string> Operation02()
        {
            return new string[] { $"Product01 - Operation 02 - {DateTime.Now}" };
        }

        [HttpGet]
        [Route("Stage01OnBehalfOfUser")]
        public async Task<IEnumerable<string>> Stage01OnBehalfOfUser()
        {
            _telemetryClient.TrackEvent("Stage01OnBehalfOfUser", new Dictionary<string, string>() { { "Time Of Invocation", DateTime.Now.ToString() } });
            List<string> claims = new List<string>();
            claims.Add("On behalf of flow");
            claims.AddRange(await InvokeProduct02APIonBehalfOfUser("Stage02"));
            return claims;
        }

        [HttpGet]
        [Route("Stage01ClientCredentialsFlow")]
        public async Task<IEnumerable<string>> Stage01ClientCredentialsFlow()
        {
            _telemetryClient.TrackEvent("Stage01ClientCredentialsFlow", new Dictionary<string, string>() { { "Time Of Invocation", DateTime.Now.ToString() } });
            List<string> claims = new List<string>();
            claims.Add("Client Credentials Flow ");//Graph API call to GetUserProfile will fail expectedly
            claims.AddRange(await InvokeProduct02APIonBehalfOfUser("Stage02"));
            return claims;
        }

        #region Application Permission Calls 
        [HttpGet]
        [Route("Stage01ReadUsingApplicationPermission")]
        public async Task<IEnumerable<string>> Stage01ReadUsingApplicationPermission()
        {
            _telemetryClient.TrackEvent("Stage01ReadUsingApplicationPermission", new Dictionary<string, string>() { { "Time Of Invocation", DateTime.Now.ToString() } });

            var claims = new List<string>(await InvokeProductAPI02ClientCredentialsFlow("Stage02"));
            claims.Insert(0, "Stage01ReadUsingApplicationPermission");
            claims.Insert(1, "=======================================");

            try
            {
               claims.AddRange(await InvokeProductAPI02ClientCredentialsFlow("ReadUsingApplicationPermission"));
            }
            catch(Exception ex)
            {
                claims.Add($"Error Stage01ReadUsingApplicationPermission - {ex.Message}");
            }
            return claims;
        }

        [HttpGet]
        [Route("Stage01WriteUsingApplicationPermission")]
        public async Task<IEnumerable<string>> Stage01WriteUsingApplicationPermission()
        {
            _telemetryClient.TrackEvent("Stage01WriteUsingApplicationPermission", new Dictionary<string, string>() { { "Time Of Invocation", DateTime.Now.ToString() } });

            var claims = new List<string>(await InvokeProductAPI02ClientCredentialsFlow("Stage02"));
            claims.Insert(0, "Stage01WriteUsingApplicationPermission");
            claims.Insert(1, "=======================================");

            try
            {
                claims.AddRange(await InvokeProductAPI02ClientCredentialsFlow("WriteUsingApplicationPermission"));
            }
            catch (Exception ex)
            {
                claims.Add($"Error Stage01WriteUsingApplicationPermission - {ex.Message}");
            }
            return claims;
        }
        #endregion

        #region Delegated Permission calls
        [HttpGet]
        [Route("Stage01ReadUsingDelegatedPermission")]
        public async Task<IEnumerable<string>> Stage01ReadUsingDelegatedPermission()
        {
            _telemetryClient.TrackEvent("Stage01ReadUsingDelegatedPermission", new Dictionary<string, string>() { { "Time Of Invocation", DateTime.Now.ToString() } });

            var claims = new List<string>(await InvokeProduct02APIonBehalfOfUser("Stage02"));
            claims.Insert(0, "Stage01ReadUsingDelegatedPermission");
            claims.Insert(1, "=======================================");

            try
            {
                claims.AddRange(await InvokeProduct02APIonBehalfOfUser("ReadUsingDelegatedPermission"));
            }
            catch (Exception ex)
            {
                claims.Add($"Error Stage01ReadUsingDelegatedPermission - {ex.Message}");
            }
            return claims;
        }

        [HttpGet]
        [Route("Stage01WriteUsingDelegatedPermission")]
        public async Task<IEnumerable<string>> Stage01WriteUsingDelegatedPermission()
        {
            _telemetryClient.TrackEvent("Stage01WriteUsingDelegatedPermission", new Dictionary<string, string>() { { "Time Of Invocation", DateTime.Now.ToString() } });

            var claims = new List<string>(await InvokeProduct02APIonBehalfOfUser("Stage02"));
            claims.Insert(0, "Stage01WriteUsingDelegatedPermission");
            claims.Insert(1, "=======================================");

            try
            {
                claims.AddRange(await InvokeProduct02APIonBehalfOfUser("WriteUsingDelegatedPermission"));
            }
            catch (Exception ex)
            {
                claims.Add($"Error Stage01WriteUsingDelegatedPermission - {ex.Message}");
            }
            return claims;
        }
        #endregion

        #region Logged in User Group based authorization (Delegated Permission calls)
        [HttpGet]
        [Route("Stage01ReadUsingViewerSecurityGroup")]
        public async Task<IEnumerable<string>> Stage01ReadUsingViewerSecurityGroup()
        {
            _telemetryClient.TrackEvent("Stage01ReadUsingViewerSecurityGroup", new Dictionary<string, string>() { { "Time Of Invocation", DateTime.Now.ToString() } });

            var claims = new List<string>(await InvokeProduct02APIonBehalfOfUser("Stage02"));
            claims.Insert(0, "Stage01ReadUsingViewerSecurityGroup");
            claims.Insert(1, "=======================================");

            try
            {
                claims.AddRange(await InvokeProduct02APIonBehalfOfUser("ReadUsingViewerSecurityGroup"));
            }
            catch (Exception ex)
            {
                claims.Add($"Error Stage01ReadUsingViewerSecurityGroup - {ex.Message}");
            }
            return claims;
        }

        [HttpGet]
        [Route("Stage01WriteUsingContributorSecurityGroup")]
        public async Task<IEnumerable<string>> Stage01WriteUsingContributorSecurityGroup()
        {
            _telemetryClient.TrackEvent("Stage01WriteUsingContributorSecurityGroup", new Dictionary<string, string>() { { "Time Of Invocation", DateTime.Now.ToString() } });

            var claims = new List<string>(await InvokeProduct02APIonBehalfOfUser("Stage02"));
            claims.Insert(0,"Stage01WriteUsingContributorSecurityGroup");
            claims.Insert(1, "=======================================");

            try
            {
                claims.AddRange(await InvokeProduct02APIonBehalfOfUser("WriteUsingContributorSecurityGroup"));
            }
            catch (Exception ex)
            {
                claims.Add($"Error Stage01WriteUsingContributorSecurityGroup - {ex.Message}");
            }
            return claims;
        }
        #endregion

        // GET: api/Product01/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return $"Get output {id}";
        }

        // POST: api/Product01
        [HttpPost]
        public string Post([FromBody] string value)
        {
            return $"Post output {value}";
        }

        // PUT: api/Product01/5
        [HttpPut("{id}")]
        public string Put(int id, [FromBody] string value)
        {
            return $"Put output {value}";
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return $"Delete output {id}";

        }

        private async Task<string[]> InvokeProduct02APIonBehalfOfUser(string operation)
        {
            try
            {
                var customerapiurl = "https://pm-apim-cp.azure-api.net/p2/api/Product02/" + operation;

                //writer.Write("Customer Api url: " + customerapiurl);
                var result = await OnBehalfOfTokenGenerator.FetchToken(_httpContextAccessor, _azureAdOptions);


                var request = (HttpWebRequest)WebRequest.Create(customerapiurl);
                request.Method = "GET";
                request.Headers["Authorization"] = string.Format("Bearer {0}", result.AccessToken);
                request.Headers["Ocp-Apim-Subscription-Key"] = "594381372f8f4c8194764a09030636cc";
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
                return JsonConvert.DeserializeObject<string[]>(content);
            }
            catch (Exception ex)
            {
                //writer.Write("InvokeCustomerAPI Error: " + ex.Message);
                return new string[] { ex.Message + " ---  " + ex.StackTrace };
            }
        }

        private async Task<string[]> InvokeProductAPI02ClientCredentialsFlow(string operation)
        {
            var bearerToken = ClientCredentialFlowTokenGenerator.FetchToken(_azureAdOptions);
            var customerapiurl = "https://pm-apim-cp.azure-api.net/p2/api/Product02/" + operation;

            //writer.Write("Customer Api url: " + customerapiurl);
            var result = await ClientCredentialFlowTokenGenerator.FetchToken(_azureAdOptions);


            var request = (HttpWebRequest)WebRequest.Create(customerapiurl);
            request.Method = "GET";
            request.Headers["Authorization"] = string.Format("Bearer {0}", result.AccessToken);
            request.Headers["Ocp-Apim-Subscription-Key"] = "594381372f8f4c8194764a09030636cc";
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
            return JsonConvert.DeserializeObject<string[]>(content);
        }

    }
}
