using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBusListener.Configuration
{
    public class BackgroundTaskSettings
    {
        public string ServiceBusConnectionString { get; set; }

        public string TopicName { get; set; }

        public string SubscriptionName { get; set; }

        public static TelemetryClient telemetryClient = new TelemetryClient { InstrumentationKey = "6e25b854-3331-471e-b898-cc6d68ef1088" };

    }
}
