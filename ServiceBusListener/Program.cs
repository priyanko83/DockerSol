using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceBusListener.MessageProcessors;
using Microsoft.Extensions.Configuration.Binder;
using ServiceBusListener.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;

namespace ServiceBusListener
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = ConfigureApplicationInsightAnTelemetry();

            IServiceBusMessageConsumer consumer = new ClaimsHandler(logger); //TODO: Use DI to inject
            IServiceBusMessageConsumer wireTap = new MessageMonitoring(logger); //TODO: Use DI to inject
            DeadLetterMessageHandler deadLetterHandler = new DeadLetterMessageHandler(logger); //TODO: Use DI to inject


            ConfigureServices(consumer, wireTap, deadLetterHandler, logger).GetAwaiter().GetResult();
        }

        // Ref: https://docs.microsoft.com/en-us/azure/azure-monitor/app/ilogger
        // Diff Between telementry & application insight: https://github.com/Microsoft/ApplicationInsights-aspnetcore/wiki/FAQs
        private static ILogger<Program> ConfigureApplicationInsightAnTelemetry()
        {
            // Create the DI container.
            IServiceCollection services = new ServiceCollection();

            // Channel is explicitly configured to do flush on it later.
            var channel = new InMemoryChannel();
            services.Configure<TelemetryConfiguration>(
                (config) =>
                {
                    config.TelemetryChannel = channel;
                }
            );

            // Add the logging pipelines to use. We are using Application Insights only here.
            services.AddLogging(builder =>
            {
                // Optional: Apply filters to configure LogLevel Trace or above is sent to
                // Application Insights for all categories.
                builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
                                 ("", LogLevel.Information);
                builder.AddApplicationInsights("6e25b854-3331-471e-b898-cc6d68ef1088"); // Pull the key from config
            });

            // Build ServiceProvider.
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();


            

            return logger;
        }

        public static async Task ConfigureServices(IServiceBusMessageConsumer consumer, 
            IServiceBusMessageConsumer wireTap, DeadLetterMessageHandler deadLetterHandler, ILogger<Program> logger)
        {
            var host = new HostBuilder()
                
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddSingleton<IHostedService, ServiceBusListenerService>();
                })
                .UseConsoleLifetime()
                .Build();
            
            using (host)
            {
                var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               // VERY IMPORTANT: Be sure to right-click this file and select properties. Change the “Copy to Output Directory” option to “Copy if newer”.
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();

                IConfigurationRoot configuration = builder.Build();
                var azureADOptions = new AzureAdOptions();
                configuration.GetSection("AzureAd").Bind(azureADOptions);


                
                // Start the host
                await host.StartAsync();
                logger.LogInformation("Monitoring started");
                consumer.StartReceivingMessages();
                wireTap.StartReceivingMessages();
                deadLetterHandler.StartReceivingMessages();
                Console.WriteLine("Job host started");

                // Wait for the host to shutdown
                await host.WaitForShutdownAsync();
            }
        }
    }
}
