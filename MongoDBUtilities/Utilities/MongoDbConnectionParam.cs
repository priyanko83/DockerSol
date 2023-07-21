using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBUtilities.Utilities
{
    public class MongoDbConnectionParam
    {
        public MongoDbConnectionParam(string connectionString, int host, string databaseName, string userName, string password)
        {
            this.ConnectionString = connectionString;
            this.Host = host;
            this.DatabaseName = databaseName;
            this.UserName = userName;
            this.Password = password;
        }

        public string ConnectionString { get; private set; }
        public int Host { get; private set; }
        public string DatabaseName { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
    }
}
