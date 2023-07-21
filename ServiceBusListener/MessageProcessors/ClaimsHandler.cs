using AzureServiceBusManager;
using Claims.Core.DTOs;
using Claims.Core.Entities;
using CQRSFramework;
using Microsoft.Azure.ServiceBus;
using MongoDBUtilities.Utilities.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Claims.Core.Commands;
using Claims.Core.DomainEvents;
using MongoDB.Driver;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ServiceBusListener.Configuration;
using AzureADAuthenticationUtilities;

namespace ServiceBusListener.MessageProcessors
{
    class ClaimsHandler : IServiceBusMessageConsumer
    {
        ServiceBusHandler MessageHandler { get; set; }
        IRepository<CorporateCustomer, Guid> EventStoreRepository;
        ILogger<Program> _logger;

        public ClaimsHandler(ILogger<Program> logger)
        {
            this._logger = logger;
            this.EventStoreRepository = new ClaimsRepository("mongodb://mongo-cosmos-for-all-apps:UPLrKxBtZGGKIwtn7CHrMyaVEMvsvFMzX7elQwJvsBCfmDhWLIurPodrKCFpG3Flrsz4E4CdQSWGw3VVdkFrZA==@mongo-cosmos-for-all-apps.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@mongo-cosmos-for-all-apps@", "ExplorerEventStore");
        }

        public void StartReceivingMessages()
        {
            var config = new ServiceBusConfiguration()
            {
                ServiceBusConnectionString = "Endpoint=sb://reliable-messaging.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=DeLDU1wjhY2cqvM7Ze10KvDceuYrCTNYUcnsyetRd70=",
                TopicName = "claim-handler",
                SubscriptionName = "CREATE-CLAIM"
            };

            this.MessageHandler = new ServiceBusHandler(config);
            this.MessageHandler.RegisterOnMessageHandlerAndReceiveMessages(ProcessMessagesAsync);
        }

        public async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            TableStorageWriter writer = new TableStorageWriter();
            
            Console.WriteLine("ProcessMessage executed");
            CommandType commandType = Enum.Parse<CommandType>(message.ContentType);
            string msg = "";
            switch (commandType)
            {
                // Filter set in Service Bus Subscription "sqlExpression": "OPERATION = 'CREATE-CLAIM'",
                case CommandType.CreateNewDeclaration:
                    CreateNewDeclaration cmdCreateNewDecl = this.MessageHandler.DeserializeMessage<CreateNewDeclaration>(message.Body);                    
                    await this.HandleAsync(cmdCreateNewDecl);
                    Console.WriteLine("ClaimsHandler-Processed Create declaration : " + cmdCreateNewDecl.Declaration.Title);
                    this._logger.LogInformation("ClaimsHandler-Processed Create declaration : " + cmdCreateNewDecl.Declaration.Title);
                    msg = "ClaimsHandler-Processed Create declaration : " + cmdCreateNewDecl.Declaration.Title;
                    writer.Write(msg);
                    break;
                // Filter set in Service Bus Subscription "sqlExpression": "OPERATION = 'EDIT-CLAIM'",
                case CommandType.UpdateDeclaration:
                    UpdateDeclaration updateDecl = this.MessageHandler.DeserializeMessage<UpdateDeclaration>(message.Body);                    
                    await this.HandleAsync(updateDecl);
                    Console.WriteLine("ClaimsHandler-Processed Update declaration : " + updateDecl.Declaration.Title);
                    this._logger.LogInformation("ClaimsHandler-Processed Update declaration : " + updateDecl.Declaration.Title);
                    msg = "ClaimsHandler-Processed Update declaration : " + updateDecl.Declaration.Title;
                    break;
                default:
                    throw new Exception("Command type could not be resolved");                    
            }
            BackgroundTaskSettings.telemetryClient.TrackEvent(msg, new Dictionary<string, string>() { { "CreateNewDeclaration", DateTime.Now.ToString() } });
           
            await this.MessageHandler.MarkMessageAsCompleteAsync(message);
            
            await SignalRNotifiationHandler.PushNotification(msg);

        }

        async Task HandleAsync(CreateNewDeclaration command)
        {
            await CommitEventsWithOptimisticLock(command, 0);
        }

        async Task HandleAsync(UpdateDeclaration command)
        {
            await CommitEventsWithOptimisticLock(command, 0);
        }        
        
        private async Task CommitEventsWithOptimisticLock(CreateNewDeclaration command, int retryAttemptCount)
        {
            try
            {
                var corpCustomer = await this.EventStoreRepository.RehydrateAggregateFromEventStream();
                var msg = string.Format("Initiating create {0}, Total declaration value {1}, Sequence {2}, Server {3}",
                    command.Declaration.Title,
                    corpCustomer.AllCorporateDeclarations.Sum(item => item.ClaimAmount), command.Sequence, Environment.MachineName);
                corpCustomer.EventLog.Add(msg);
                this._logger.LogInformation(msg);
                await SignalRNotifiationHandler.PushNotification(msg);
                corpCustomer.CreateDeclaration(EventMapper.TransformIntoDomainEvent(command, corpCustomer.Version + 1));

                if (corpCustomer.IsValidAggregate())
                {
                    try
                    {
                        await this.EventStoreRepository.CommitChanges(corpCustomer.EventToCommit);
                        msg = string.Format("SUCCESS: Create {0}", command.Declaration.Title);
                        corpCustomer.EventLog.Add(msg);
                        this._logger.LogInformation(msg);
                        await SignalRNotifiationHandler.PushNotification(msg);


                        msg = string.Format("Time elapsed for {0} - Seq:{1} equals {2} milisseconds",
                    command.Declaration.Title, command.Sequence, (DateTime.UtcNow - command.CommandInitiationTimeUtc).TotalMilliseconds);
                        corpCustomer.EventLog.Add(msg);
                        this._logger.LogInformation(msg);
                        await this.EventStoreRepository.LogAsync(String.Join(",  ", corpCustomer.EventLog));
                        await SignalRNotifiationHandler.PushNotification(msg);

                    }
                    catch (MongoWriteException mex)
                    {
                        if (mex.Message.Contains("E11000 duplicate key error"))
                        {
                            retryAttemptCount++;
                            msg = string.Format("DUPLICATEVERSION.RETRY create {0},seq{1},Attempt{2},Server:{3}",
                                command.Declaration.Title, command.Sequence, retryAttemptCount, Environment.MachineName);
                            await this.EventStoreRepository.LogAsync(msg);
                            this._logger.LogInformation(msg);
                            await SignalRNotifiationHandler.PushNotification(msg);


                            if (retryAttemptCount <= 5)
                            {
                                await CommitEventsWithOptimisticLock(command, retryAttemptCount);
                            }
                        }
                        else
                        {
                            string str = mex.Message;
                            if (mex.InnerException != null)
                            {
                                str += " Inner Exception = " + mex.InnerException;
                            }
                            //await this.EventStoreRepository.LogAsync(str);
                            this._logger.LogInformation(str);
                            await SignalRNotifiationHandler.PushNotification(str);
                        }
                    }
                }
                else
                {
                    msg = string.Format("FAILED: Create {0}", command.Declaration.Title);
                    corpCustomer.EventLog.Add(msg);
                    this._logger.LogError(msg);
                    await SignalRNotifiationHandler.PushNotification(msg);

                    msg = string.Format("Time elapsed for {0}-Seq{1} equals {2} milisseconds",
                    command.Declaration.Title, command.Sequence, (DateTime.UtcNow - command.CommandInitiationTimeUtc).TotalMilliseconds);
                    corpCustomer.EventLog.Add(msg);
                    this._logger.LogInformation(msg);
                    await this.EventStoreRepository.LogAsync(String.Join(",  ", corpCustomer.EventLog));
                    await SignalRNotifiationHandler.PushNotification(msg);

                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                if (ex.InnerException != null)
                {
                    str += " Inner Exception = " + ex.InnerException;
                }
                await this.EventStoreRepository.LogAsync(str);
                this._logger.LogInformation(str);
                await SignalRNotifiationHandler.PushNotification(str);

            }
        }

        private async Task CommitEventsWithOptimisticLock(UpdateDeclaration command, int retryAttemptCount)
        {
            try
            {
                var corpCustomer = await this.EventStoreRepository.RehydrateAggregateFromEventStream();
                var msg = string.Format("Init update {0},Totalvalue:{1},Seq:{2},Server:{3}",
                    command.Declaration.Title,
                    corpCustomer.AllCorporateDeclarations.Sum(item => item.ClaimAmount), command.Sequence, Environment.MachineName);
                corpCustomer.EventLog.Add(msg);
                this._logger.LogInformation(msg);
                corpCustomer.UpdateDeclaration(EventMapper.TransformIntoDomainEvent(command, corpCustomer.Version + 1));
                await SignalRNotifiationHandler.PushNotification(msg);

                if (corpCustomer.IsValidAggregate())
                {
                    try
                    {
                        await this.EventStoreRepository.CommitChanges(corpCustomer.EventToCommit);
                        msg = string.Format("SUCCESS: Edit {0}", command.Declaration.Title);
                        corpCustomer.EventLog.Add(msg);
                        this._logger.LogInformation(msg);
                        await SignalRNotifiationHandler.PushNotification(msg);

                        msg = string.Format("Time elapsed for {0},Seq{1} equals {2} milisseconds",
                    command.Declaration.Title, command.Sequence, (DateTime.UtcNow - command.CommandInitiationTimeUtc).TotalMilliseconds);
                        corpCustomer.EventLog.Add(msg);
                        this._logger.LogInformation(msg);
                        await this.EventStoreRepository.LogAsync(String.Join(",  ", corpCustomer.EventLog)); 
                        await SignalRNotifiationHandler.PushNotification(msg);
                    }
                    catch (MongoWriteException mex)
                    {
                        if (mex.Message.Contains("E11000 duplicate key error"))
                        {
                            retryAttemptCount++;

                            msg = string.Format("DUPLICATEVERSION.RETRY update{0},seq{1}.Attempt{2}.Server:{3}",
                                command.Declaration.Title, command.Sequence, retryAttemptCount, Environment.MachineName);
                            await this.EventStoreRepository.LogAsync(msg);
                            this._logger.LogInformation(msg);
                            await SignalRNotifiationHandler.PushNotification(msg);

                            if (retryAttemptCount <= 5)
                            {
                                await CommitEventsWithOptimisticLock(command, retryAttemptCount);
                            }
                        }
                    }
                }
                else
                {
                    msg = string.Format("FAILED: Edit {0}", command.Declaration.Title);
                    corpCustomer.EventLog.Add(msg);
                    this._logger.LogInformation(msg);
                    await SignalRNotifiationHandler.PushNotification(msg);

                    msg = string.Format("Total time elapsed in processing command {0} - Sequence {1} equals {2} milisseconds",
                    command.Declaration.Title, command.Sequence, (DateTime.UtcNow - command.CommandInitiationTimeUtc).TotalMilliseconds);
                    corpCustomer.EventLog.Add(msg);
                    await this.EventStoreRepository.LogAsync(String.Join(",  ", corpCustomer.EventLog));
                    this._logger.LogInformation(msg);
                    await SignalRNotifiationHandler.PushNotification(msg);
                }

            }
            catch (Exception ex)
            {
                string str = ex.Message;
                if (ex.InnerException != null)
                {
                    str += " Inner Exception = " + ex.InnerException;
                }
                await this.EventStoreRepository.LogAsync(str);
                this._logger.LogInformation(str);
                await SignalRNotifiationHandler.PushNotification("Error CommitEventsWithOptimisticLock:" + str );

            }
        }
    }
}
