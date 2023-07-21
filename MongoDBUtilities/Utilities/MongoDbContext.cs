using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBUtilities.Utilities
{
    public class MongoDbContext
    {
        public IMongoDatabase Database;

        public MongoDbContext(MongoClientSettings settings, string databaseName)
        {            
            Database = new MongoClient(settings).GetDatabase(databaseName);
        }                   
    }
}
