using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureServiceBusManager
{
    public class ServiceBusLog : TableEntity
    {
        public ServiceBusLog()
        {
            this.PartitionKey = "Logs";
            this.RowKey = Guid.NewGuid().ToString();
        }

        public ServiceBusLog(string text)
        {
            this.PartitionKey = "Logs";
            this.RowKey = Guid.NewGuid().ToString();
            this.LogText = text;
        }

        public string LogText { get; set; }
    }
}
