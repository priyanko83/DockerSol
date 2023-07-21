using AzureADAuthenticationUtilities;
using GatewayApi2.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace GatewayApi2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class SignalRHubController : ControllerBase
    {
        private readonly ILogger<SignalRHubController> _logger;
        private IHubContext<NotifyHub> _hubContext;

        public SignalRHubController(ILogger<SignalRHubController> logger, IHubContext<NotifyHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<string> PostData(CustomMessage msg)
        {
            TableStorageWriter writer = new TableStorageWriter();
            try
            {
                
                writer.Write("Signalrhub post: " + JsonConvert.SerializeObject(msg));
                //return Clients.User(user).SendAsync("ReceiveMessage", message);
                //await _hubContext.Clients.User("viewer01@priyankomukherjeegmail.onmicrosoft.com")
                await _hubContext.Clients.Group("viewer01@priyankomukherjeegmail.onmicrosoft.com")
                .SendAsync("MessageReceived",
                new CustomMessage()
                {
                    clientuniqueid = DateTime.Now.ToLongTimeString(),
                    message = msg.message
                });


                return msg.message;
            }
            catch(Exception ex)
            {
                writer.Write("Signalrhub post error: " +ex.Message + (ex.InnerException == null ? "" : ex.InnerException.Message));
                throw;
            }
        }
    }

    public class CustomMessage
    {
        public string clientuniqueid { get; set; }
        public string message { get; set; }
    }
}