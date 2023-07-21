using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace SignalRHub.Controllers
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

        [HttpGet]
        public async Task<string> Get()
        {
            await _hubContext.Clients.All.SendAsync("MessageReceived",
                new Message()
                {
                    clientuniqueid = DateTime.Now.ToLongTimeString(),
                    message = "Message sent at: " + DateTime.Now.ToLongTimeString()
                });
            return "Broadcasted the messsage";
        }

        [HttpPost]
        public async Task<string> Post(Message msg)
        {
            //return Clients.User(user).SendAsync("ReceiveMessage", message);
            await _hubContext.Clients.User("viewer01@priyankomukherjeegmail.onmicrosoft.com").SendAsync("MessageReceived",
                new Message()
                {
                    clientuniqueid = DateTime.Now.ToLongTimeString(),
                    message = msg.message
                });
            return "Broadcasted the messsage";
        }
    }
}
