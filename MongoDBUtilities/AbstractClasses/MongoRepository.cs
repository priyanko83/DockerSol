using Claims.Core.DomainEvents;
using CQRSFramework;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDBUtilities.Utilities;
using System;
using System.Security.Authentication;

namespace MongoDBUtilities.AbstractClasses
{
    public abstract class MongoRepository 
    {
        public readonly IMongoDatabase MongoDatabase;

        static MongoRepository()
        {
            BsonClassMap.RegisterClassMap<DeclarationCreated>();
            BsonClassMap.RegisterClassMap<DeclarationUpdated>();
            BsonClassMap.RegisterClassMap<NewIncidentAdded>();
            BsonClassMap.RegisterClassMap<IncidentUpdated>();
            BsonClassMap.RegisterClassMap<IncidentDeleted>();
        }

        protected MongoRepository(string primaryConnectionString, string databaseName)
        {            
            var client = new MongoClient(primaryConnectionString);
            MongoDatabase = client.GetDatabase(databaseName);
        }
    }
}
