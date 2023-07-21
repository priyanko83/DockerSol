using AzureADAuthenticationUtilities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayApi2.SignalR
{
    public class NotifyHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            TableStorageWriter writer = new TableStorageWriter();

            try
            {
                if(Context!= null && Context.User != null && Context.User.Identity != null)
                {
                    writer.Write("OnConnectedAsync Initiate:" + Context.User.Identity.Name);
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
                await base.OnConnectedAsync();
                writer.Write("OnConnectedAsync Success:" + Context.User.Identity.Name);
            }
            catch(Exception ex)
            {
                writer.Write("OnConnectedAsync Exception:" + ex.Message);
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            TableStorageWriter writer = new TableStorageWriter();

            try
            {
                if (Context != null && Context.User != null && Context.User.Identity != null)
                {
                    writer.Write("OnDisconnectedAsync Initiate:" + Context.User.Identity.Name);
                }
                writer.Write("OnDisconnectedAsync Initiate:" + ex.Message);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
                await base.OnDisconnectedAsync(ex);
            }
            catch (Exception e)
            {
                writer.Write("OnDisconnectedAsync Exception:" + e.Message);
                throw;
            }
        }
    }
}
