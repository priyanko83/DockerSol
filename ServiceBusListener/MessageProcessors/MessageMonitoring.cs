using AzureServiceBusManager;
using Claims.Core.Commands;
using Claims.Core.Entities;
using CQRSFramework;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using MongoDBUtilities.Utilities.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusListener.MessageProcessors
{
    class MessageMonitoring : IServiceBusMessageConsumer
    {
        ServiceBusHandler MessageHandler { get; set; }
        ILogger<Program> _logger;

        /// <summary>
        /// No Filter set in Service Bus Subscription "WIRE-TAP", This is used to monitor and perform reporting
        /// </summary>
        public MessageMonitoring(ILogger<Program> logger)
        {
            this._logger = logger;
        }

        public void StartReceivingMessages()
        {
            var config = new ServiceBusConfiguration()
            {                
                ServiceBusConnectionString = "Endpoint=sb://reliable-messaging.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=DeLDU1wjhY2cqvM7Ze10KvDceuYrCTNYUcnsyetRd70=",
                TopicName = "claim-handler",
                SubscriptionName = "WIRE-TAP"
            };

            this.MessageHandler = new ServiceBusHandler(config);
            this.MessageHandler.RegisterOnMessageHandlerAndReceiveMessages(ProcessMessagesAsync);
        }

        public async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            CommandType commandType = Enum.Parse<CommandType>(message.ContentType);

            switch (commandType)
            {
                case CommandType.CreateNewDeclaration:
                    CreateNewDeclaration cmdCreateNewDecl = this.MessageHandler.DeserializeMessage<CreateNewDeclaration>(message.Body);
                    Console.WriteLine("WireTap-Processed Create declaration : " + cmdCreateNewDecl.Declaration.Title);
                    break;
                case CommandType.UpdateDeclaration:
                    UpdateDeclaration cmdUpdateDecl = this.MessageHandler.DeserializeMessage<UpdateDeclaration>(message.Body);
                    Console.WriteLine("WireTap-Processed Update declaration : " + cmdUpdateDecl.Declaration.Title);
                    break;
                default:
                    throw new Exception("Command type could not be resolved");
            }
            await this.MessageHandler.MarkMessageAsCompleteAsync(message);
        }

    }
}
