using AzureServiceBusManager;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusListener
{
    interface IServiceBusMessageConsumer
    {
        void StartReceivingMessages();
        Task ProcessMessagesAsync(Message message, CancellationToken token);
    }
}
