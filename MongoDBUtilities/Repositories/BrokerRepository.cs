using CQRSFramework;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBUtilities.AbstractClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDBUtilities.Utilities.Repositories
{
    public class BrokerRepository : MongoRepository
    {
        // TODO : Move these to Azure settings and pass these from executable project (Azure app service or web job)
        private const string CollectionName = "Appointments";
        private IMongoCollection<BsonDocument> _logsCollection;
        
        public BrokerRepository(string onPremiseConnString, string databaseName) : base(onPremiseConnString, databaseName)
        {
            _logsCollection = MongoDatabase.GetCollection<BsonDocument>("ApplicationLogs");
        }

        public async Task Add(ApplicationCommand command)
        {
            var insuranceBrokerCollection = GetEventStoreCollection();
            await insuranceBrokerCollection.InsertOneAsync(command);
        }        

        public async Task LogAsync(string log)
        {
            var documnt = new BsonDocument
            {
                {"log",log}
            };
            await _logsCollection.InsertOneAsync(documnt);
        }

        private IMongoCollection<ApplicationCommand> GetEventStoreCollection()
        {
            return MongoDatabase.GetCollection<ApplicationCommand>(BrokerRepository.CollectionName);
        }        
    }
}
