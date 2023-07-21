using AzureServiceBusManager;
using Claims.Core.Commands;
using CQRSFramework;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusListener.MessageProcessors
{
    class DeadLetterMessageHandler 
    {
        ServiceBusHandler CreateClaimSubscriptionDeadletterHandler { get; set; }
        ServiceBusHandler EditClaimSubscriptionDeadletterHandler { get; set; }
        ServiceBusHandler MessageMonitoringSubscriptionDeadletterHandler { get; set; }
        ILogger<Program> _logger;

        public DeadLetterMessageHandler(ILogger<Program> logger)
        {
            this._logger = logger;
        }

        public void StartReceivingMessages()
        {           
            this.CreateClaimSubscriptionDeadletterHandler = new ServiceBusHandler(FetchDeadletterSubscriptionConfiguration("CREATE-CLAIM"));
            this.CreateClaimSubscriptionDeadletterHandler.RegisterOnMessageHandlerAndReceiveMessages(ProcessCreateClaimSubscriptionMessagesAsync);

            this.EditClaimSubscriptionDeadletterHandler = new ServiceBusHandler(FetchDeadletterSubscriptionConfiguration("EDIT-CLAIM"));
            this.EditClaimSubscriptionDeadletterHandler.RegisterOnMessageHandlerAndReceiveMessages(ProcessEditClaimSubscriptionMessagesAsync);

            this.MessageMonitoringSubscriptionDeadletterHandler = new ServiceBusHandler(FetchDeadletterSubscriptionConfiguration("WIRE-TAP"));
            this.MessageMonitoringSubscriptionDeadletterHandler.RegisterOnMessageHandlerAndReceiveMessages(ProcessMessageMonitoringSubscriptionMessagesAsync);
        }

        public async Task ProcessCreateClaimSubscriptionMessagesAsync(Message message, CancellationToken token)
        {
            await ProcessMessagesAsync(message, token, this.CreateClaimSubscriptionDeadletterHandler, "Deadletter messages in Create Claim Subscription");
        }

        public async Task ProcessEditClaimSubscriptionMessagesAsync(Message message, CancellationToken token)
        {
            await ProcessMessagesAsync(message, token, this.EditClaimSubscriptionDeadletterHandler, "Deadletter messages in Edit Claim Subscription");
        }

        public async Task ProcessMessageMonitoringSubscriptionMessagesAsync(Message message, CancellationToken token)
        {
            await ProcessMessagesAsync(message, token, this.MessageMonitoringSubscriptionDeadletterHandler, "Deadletter messages in Monitor Messages Subscription");
        }

        public async Task ProcessMessagesAsync(Message message, CancellationToken token, ServiceBusHandler handler, string subscriptionChannel)
        {
            CommandType commandType = Enum.Parse<CommandType>(message.ContentType);

            switch (commandType)
            {
                // Filter set in Service Bus Subscription "sqlExpression": "OPERATION = 'CREATE-CLAIM'",
                case CommandType.CreateNewDeclaration:
                    CreateNewDeclaration cmdCreateNewDecl = this.CreateClaimSubscriptionDeadletterHandler.DeserializeMessage<CreateNewDeclaration>(message.Body);                    
                    Console.WriteLine(subscriptionChannel + "=> " + message.UserProperties["OPERATION"] + " Create declaration : " + cmdCreateNewDecl.Declaration.Title);
                    this._logger.LogInformation(subscriptionChannel + "=> " + message.UserProperties["OPERATION"] + " Create declaration : " + cmdCreateNewDecl.Declaration.Title);
                    await handler.MarkMessageAsCompleteAsync(message);
                    break;
                // Filter set in Service Bus Subscription "sqlExpression": "OPERATION = 'EDIT-CLAIM'",
                case CommandType.UpdateDeclaration:
                    UpdateDeclaration updateDecl = this.EditClaimSubscriptionDeadletterHandler.DeserializeMessage<UpdateDeclaration>(message.Body);
                    Console.WriteLine(subscriptionChannel + "=> " + message.UserProperties["OPERATION"] + " Update declaration : " + updateDecl.Declaration.Title);
                    await handler.MarkMessageAsCompleteAsync(message);
                    break;
                default:
                    throw new Exception("Command type could not be resolved");
            }
        }

        private ServiceBusConfiguration FetchDeadletterSubscriptionConfiguration(string subscriptionName)
        {
            return new ServiceBusConfiguration()
            {
                ServiceBusConnectionString = "Endpoint=sb://reliable-messaging.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=DeLDU1wjhY2cqvM7Ze10KvDceuYrCTNYUcnsyetRd70=",
                TopicName = "claim-handler",
                SubscriptionName = subscriptionName + "/$DeadLetterQueue"
            };
        }
    }
}
