using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI
{
    public class TableStorageWriter
    {
        // private property  
        private CloudTable table;
        private CloudTableClient tableClient;

        // Constructor   
        public TableStorageWriter()
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=priyankoazure;AccountKey=rtlzbN1fdbxAsjO7I9DVH73ojco07BhRbWeDawNSb3xC+q6ZTeoNAUyrKzBkUKxd45IegzG9rKAk1BgB6dB9xQ==;EndpointSuffix=core.windows.net");
                tableClient = storageAccount.CreateCloudTableClient();

                table = tableClient.GetTableReference("microservices");
                table.CreateIfNotExistsAsync().Wait();
            }
            catch
            {

            }
        }
        
        public void Write(string message)
        {
            try
            {
                var newEntity = new LogEntity()
                {
                    PartitionKey = "None",
                    RowKey = Guid.NewGuid().ToString(),
                    Log = message
                };

                var insertOrMergeOperation = TableOperation.InsertOrReplace(newEntity);
                table.ExecuteAsync(insertOrMergeOperation).Wait();
            }
            catch
            {
                
            }
        }

    }
    public class LogEntity : TableEntity
    {
        public string Log { get; set; }
    }
}
