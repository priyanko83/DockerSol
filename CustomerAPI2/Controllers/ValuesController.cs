using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AzureADAuthenticationUtilities.GraphApi;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace CustomerAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IGraphApiService _graphApiService;
        private readonly TelemetryClient _telemetryClient;

        public ValuesController(ILogger<ValuesController> logger, IGraphApiService graphApiService, TelemetryClient telemetryClient)
        {
            _graphApiService = graphApiService;
            _logger = logger;
            _telemetryClient = telemetryClient;
        }

        // GET api/values
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            try
            {
                _telemetryClient.TrackEvent("", new Dictionary<string, string>() { { "CustomerAPI.GenerateTestData", DateTime.Now.ToString() } });
                _logger.LogInformation("This is test log from customer api at " + DateTime.Now);
                CommandsGenerator g = new CommandsGenerator();
                await g.CreateTestDeclarationsAsync();
            }
            catch (Exception ex)
            {
                TableStorageWriter writer = new TableStorageWriter();
                writer.Write("Customer api => " + ex.Message + "  === " + ex.StackTrace);
            }
            string useremail = "";
            
            try
            {
                var userProfile = await _graphApiService.GetUserProfileAsync();
                useremail = userProfile.UserPrincipalName;

            }
            catch (Exception ex)
            {
                useremail = ex.Message;
            }
            return new string[] {
                "CustomerAPIiiii --- CustomerAPI machine name: " + Environment.MachineName + " --- " + DateTime.Now.ToString("yyyyMMddHHmmss") ,
                 "<br />Graph Api called successfully: " + useremail,
                 "<br />Successfully sent messages " };
        }

    }

}