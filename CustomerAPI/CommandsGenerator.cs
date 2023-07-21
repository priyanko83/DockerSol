using AzureServiceBusManager;
using Claims.Core.Commands;
using Claims.Core.DTOs;
using Claims.Core.Entities;
using CQRSFramework;
using MongoDBUtilities.Utilities.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerAPI
{
    public class CommandsGenerator
    {
        int _sequence;
        ServiceBusHandler MessageSender { get; set; }
        IRepository<CorporateCustomer, Guid> EventStoreRepository;
        public CommandsGenerator()
        {
            var config = new ServiceBusConfiguration()
            {

                ServiceBusConnectionString = "Endpoint=sb://reliable-messaging.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=DeLDU1wjhY2cqvM7Ze10KvDceuYrCTNYUcnsyetRd70=",
                TopicName = "claim-handler"
            };

            this.MessageSender = new ServiceBusHandler(config);
            this.EventStoreRepository = new ClaimsRepository("mongodb://mongo-cosmos-for-all-apps:UPLrKxBtZGGKIwtn7CHrMyaVEMvsvFMzX7elQwJvsBCfmDhWLIurPodrKCFpG3Flrsz4E4CdQSWGw3VVdkFrZA==@mongo-cosmos-for-all-apps.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@mongo-cosmos-for-all-apps@", "ExplorerEventStore");
        }

        public async Task CreateTestDeclarationsAsync()
        {
            await this.EventStoreRepository.ClearLogAsync();
            List<CreateNewDeclaration> declarations = new List<CreateNewDeclaration>();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 12; i++)
            {
                declarations.Add(CreateNewDeclaration(new Random().Next(0, 100)));
                Console.WriteLine("Sending Create " + declarations[i].Declaration.Title);
                tasks.Add(this.MessageSender.SendAsync(declarations[i], "CREATE-CLAIM"));
            }

            await Task.WhenAll(tasks);

        }


        public async Task UpdateTestDeclarationsAsync()
        {
            await this.EventStoreRepository.ClearLogAsync();
            CorporateCustomer corpCustomer = await this.EventStoreRepository.RehydrateAggregateFromEventStream();
            UpdateDeclaration cmdUpdateDecl;
            DeclarationDTO declDTO;
            IncidentDTO incDTO;
            List<IncidentDTO> incDTOs;
            List<Task> tasks = new List<Task>();
            foreach (Declaration decl in corpCustomer.AllCorporateDeclarations)
            {
                incDTOs = new List<IncidentDTO>();
                declDTO = new DeclarationDTO()
                {
                    DeclarationId = decl.Id.ToString(),
                    Title = decl.Title
                };
                foreach (Incident inc in decl.Incidents)
                {
                    incDTO = new IncidentDTO()
                    {
                        IncidentId = inc.Id.ToString(),
                        TypeOfChange = TypeOfModification.Delete
                    };
                    incDTOs.Add(incDTO);
                }

                for (int i = 0; i < 3; i++)
                {
                    incDTO = new IncidentDTO(declDTO.DeclarationId, Guid.NewGuid().ToString(), string.Format("Re-creating Incident {0}----{1}", i + 1, decl.Title),
                        DateTime.Now.AddDays(-new Random().Next(0, 30)), new Random().Next(0, 100), TypeOfModification.Create);
                    incDTOs.Add(incDTO);
                }

                declDTO.Incidents = incDTOs.ToArray();
                _sequence++;
                cmdUpdateDecl = new UpdateDeclaration(declDTO) { Sequence = _sequence };
                tasks.Add(this.MessageSender.SendAsync(cmdUpdateDecl, "EDIT-CLAIM"));
            }

            foreach (Task t in tasks)
            {
                await t;
            }
        }

        private CreateNewDeclaration CreateNewDeclaration(int count)
        {
            Guid declId = Guid.NewGuid();

            string title = string.Format("Declaration {0}", count);
            _sequence++;
            List<IncidentDTO> incidents = new List<IncidentDTO>();
            IncidentDTO inc;
            for (int i = 0; i < 3; i++)
            {
                inc = new IncidentDTO(declId.ToString(), Guid.NewGuid().ToString(), string.Format("Incident {0}----{1}", count, i + 1),
                    DateTime.Now.AddDays(new Random().Next(0, 30)), new Random().Next(0, 100), TypeOfModification.Create);
                incidents.Add(inc);
            }

            DeclarationDTO declDTO = new DeclarationDTO()
            {
                DeclarationId = declId.ToString(),
                Title = title,
                Incidents = incidents.ToArray()
            };

            CreateNewDeclaration cmdDecl = new CreateNewDeclaration(declDTO)
            {
                CommandInitiationTimeUtc = DateTime.UtcNow,
                Sequence = _sequence,
                Declaration = declDTO
            };

            return cmdDecl;
        }
    }
}
