using Claims.Core.Entities;
using CQRSFramework;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBUtilities.AbstractClasses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDBUtilities.Utilities.Repositories
{
    public class ClaimsRepository : MongoRepository, IRepository<CorporateCustomer, Guid>
    {
        // TODO : Move these to Azure settings and pass these from executable project (Azure app service or web job)
        private const string CollectionName = "ClaimEventStore";
        private IMongoCollection<BsonDocument> _logsCollection;
        
        public ClaimsRepository(string onPremiseConnString, string databaseName) : base(onPremiseConnString, databaseName)
        {
            _logsCollection = MongoDatabase.GetCollection<BsonDocument>("ApplicationLogs");
        }

        public async Task CommitChanges(DomainEvent eventToCommit)
        {
            var eventsCollection = GetEventStoreCollection();

            await eventsCollection.InsertOneAsync(eventToCommit);
        }

        public async Task<CorporateCustomer> RehydrateAggregateFromEventStream()
        {
            var pastEvents = await LoadEventStream();
            return new CorporateCustomer(Guid.NewGuid(), pastEvents);
        }


        private async Task<List<DomainEvent>> LoadEventStream()
        {
            try
            {
                List<DomainEvent> pastEvents = new List<DomainEvent>();
                var eventStoreCollection = GetEventStoreCollection();
                using (IAsyncCursor<DomainEvent> cursor = await eventStoreCollection.FindAsync(new BsonDocument()))
                {
                    while (await cursor.MoveNextAsync())
                    {
                        IEnumerable<DomainEvent> batch = cursor.Current;
                        foreach (DomainEvent @event in batch)
                        {
                            pastEvents.Add(@event);
                        }
                    }
                }

                return pastEvents;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task LogAsync(string log)
        {            
            var documnt = new BsonDocument
            {
                {"log",log},
                { "Time", DateTime.Now }
            };
            await _logsCollection.InsertOneAsync(documnt);
        }

        public async Task ClearLogAsync()
        {
            var eventsCollection = GetEventStoreCollection();

            await eventsCollection.DeleteManyAsync(new BsonDocument());
            await _logsCollection.DeleteManyAsync(new BsonDocument());            
        }

        private IMongoCollection<DomainEvent> GetEventStoreCollection()
        {
            return MongoDatabase.GetCollection<DomainEvent>(ClaimsRepository.CollectionName);
        }        
    }
}
