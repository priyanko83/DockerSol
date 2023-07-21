using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AzureADAuthenticationUtilities.GraphApi;
using AzureServiceBusManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace CustomerAPI.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ILogger _logger;
        private readonly EnvironmentConfig _configuration;
        private readonly IGraphApiService _graphApiService;

        public ValuesController(IOptions<EnvironmentConfig> configuration, ILogger<ValuesController> logger, IGraphApiService graphApiService)
        {
            _configuration = configuration.Value;
            _graphApiService = graphApiService;
            _logger = logger;
        }

        // GET api/values
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            try
            {
                _logger.LogInformation("This is test log from customer api at " + DateTime.Now);
                CommandsGenerator g = new CommandsGenerator();
                await g.CreateTestDeclarationsAsync();
            }
            catch(Exception ex)
            {
                TableStorageWriter writer = new TableStorageWriter();
                writer.Write("Customer api => " + ex.Message);
            }
            string useremail = "";
            try
            {
                var userProfile = await _graphApiService.GetUserProfileAsync();
                useremail = userProfile.UserPrincipalName;
            }
            catch(Exception ex)
            {
                useremail = ex.Message;
            }
            return new string[] {
                "CustomerAPI --- CustomerAPI machine name: " + Environment.MachineName + " --- " + DateTime.Now.ToString("yyyyMMddHHmmss") + " --- CustomerApiUrl from config.. : " + _configuration.CustomerApiUrl,
                 "<br />Graph Api called successfully: " + useremail,
                 "<br />Successfully sent messages"
            };
        }


        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
            CommandsGenerator g = new CommandsGenerator();
            await g.CreateTestDeclarationsAsync();
            return "Successfully sent messages--- " + Environment.MachineName + " --- " + DateTime.Now.ToString("yyyyMMddHHmmss") + " --- Customerapi from config : " + FetchConfiguration();
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

        public string FetchConfiguration()
        {
            try
            {
                return _configuration.CustomerApiUrl;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        async Task PerformLoadTests()
        {
            string[] availableBrokerIds = new string[5] {
                "001A8FB8-78AC-4541-9797-9D589FF1F8ED",
                "002A8FB8-78AC-4541-9797-9D589FF1F8ED",
                "003A8FB8-78AC-4541-9797-9D589FF1F8ED",
                "004A8FB8-78AC-4541-9797-9D589FF1F8ED",
                "005A8FB8-78AC-4541-9797-9D589FF1F8ED" };

            MessageSenderTester tester = new MessageSenderTester();
            for (int i = 0; i < 5; i++)
            {
                var message = MessageSenderTester.CreateTestAppointment(availableBrokerIds, i + 1);
                await tester.SendMessageAsync(message);
            }
        }
    }
}
