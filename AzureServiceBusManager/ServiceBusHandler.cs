using CQRSFramework;
using CQRSFramework.Commands.Brokers;
using Microsoft.Azure.ServiceBus;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace AzureServiceBusManager
{
    public class ServiceBusHandler
    {        
        TopicClient TopicClientInstance;
        SubscriptionClient SubscriptionClientInstance;
        CloudTable LogTable;

        public ServiceBusHandler(ServiceBusConfiguration config)
        {            
            if (!string.IsNullOrEmpty(config.SubscriptionName))
            {
                this.SubscriptionClientInstance = new SubscriptionClient(config.ServiceBusConnectionString,
                    config.TopicName, config.SubscriptionName);
            }
            else
            {
                this.TopicClientInstance = new TopicClient(config.ServiceBusConnectionString, config.TopicName);
            }
            InitializeConnectionToAzureTableStorageAsync().GetAwaiter().GetResult();
        }

        #region Send functionalities
        public async Task SendAsync(ApplicationCommand command, string operation) // TODO: Input has to be custom class (ApplicationCommand)
        {
            try
            {                
                // Create a new message to send to the topic
                var message = new Message(SerializeMessage<ApplicationCommand>(command));
                message.ContentType = command.TypeOfCommand.ToString();
                // Enters filter for the various operations. Each operation has a dedicated subscription attached to it.
                message.UserProperties.Add("OPERATION", operation);
                
                // Send the message to the topic
                await this.TopicClientInstance.SendAsync(message);
                await LogMessageAsync($"Successfully sent message. Sequence: {command.Sequence}, Id: {command.CommandId} ");
            }
            catch (Exception exception)
            {
                await LogMessageAsync($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
        #endregion

        #region Receive functionalities
        public void RegisterOnMessageHandlerAndReceiveMessages(Func<Message, CancellationToken, Task> handler)
        {
            // Ensure default rule exists
            //await this.SubscriptionClientInstance.RemoveRuleAsync(RuleDescription.DefaultRuleName);
            //await this.SubscriptionClientInstance.AddRuleAsync(new RuleDescription(RuleDescription.DefaultRuleName, new TrueFilter()));

            // Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false
            };

            // Register the function that will process messages
            this.SubscriptionClientInstance.RegisterMessageHandler(handler, messageHandlerOptions);
        }

        // Use this Handler to look at the exceptions received on the MessagePump
        async Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            await LogMessageAsync($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
           
        }
        
        public async Task MarkMessageAsCompleteAsync(Message message)
        {
            //await LogMessageAsync($"Mark message as complete: {message.ContentType}");
            // Complete the message so that it is not received again.
            // This can be done only if the subscriptionClient is created in ReceiveMode.PeekLock mode (which is default).
            await this.SubscriptionClientInstance.CompleteAsync(message.SystemProperties.LockToken);
        }

        public async Task CloseSubscriptionClient()
        {
            await this.SubscriptionClientInstance.CloseAsync();
        }
        #endregion

        async Task InitializeConnectionToAzureTableStorageAsync()
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse
            ("DefaultEndpointsProtocol=https;AccountName=priyankoazure;AccountKey=rtlzbN1fdbxAsjO7I9DVH73ojco07BhRbWeDawNSb3xC+q6ZTeoNAUyrKzBkUKxd45IegzG9rKAk1BgB6dB9xQ==;EndpointSuffix=core.windows.net");

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            this.LogTable = tableClient.GetTableReference("ServiceBusTopicPerformance");

            // Create the table if it doesn't exist.
            await this.LogTable.CreateIfNotExistsAsync();
        }

        public async Task LogMessageAsync(string text)
        {
            // Create a new customer entity.
            ServiceBusLog log = new ServiceBusLog(text);

            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(log);

            Console.WriteLine(text);

            // Execute the insert operation.
            await this.LogTable.ExecuteAsync(insertOperation);
        }

        public byte[] SerializeMessage<T>(T message)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, message);
                return ms.ToArray();
            }
        }

        public T DeserializeMessage<T>(byte[] serializedMessage)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(serializedMessage, 0, serializedMessage.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                T obj = (T)binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}
