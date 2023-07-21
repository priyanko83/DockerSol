using System;
using System.Collections.Generic;
using System.Text;

namespace AzureServiceBusManager
{
    public class ServiceBusConfiguration
    {
        public string ServiceBusConnectionString { get; set; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }

    }
}
