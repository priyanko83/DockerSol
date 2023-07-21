using AzureServiceBusManager;
using CQRSFramework;
using CQRSFramework.Commands.Brokers;
using System;
using System.Threading.Tasks;

namespace CustomerAPI2
{
    public class MessageSenderTester
    {
        private ServiceBusHandler MessageSenderAgent;
        
        public MessageSenderTester()
        {
            var config = new ServiceBusConfiguration()
            {
                ServiceBusConnectionString = "Endpoint=sb://priyankoazureservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=wZPHc5AZtGIIEDNV3d1Tnuc/IJsuW/li3HY6LIz93I0=",
                TopicName = "topic001"
            };

            this.MessageSenderAgent = new ServiceBusHandler(config);                        
        }

        public async Task SendMessageAsync(ApplicationCommand command)
        {
            await this.MessageSenderAgent.SendAsync(command, "");
        }

        public static ScheduleAppointment CreateTestAppointment(string[] availableBrokerIds, int sequence)
        {
            string uniqueText = "Appointment created At - " + DateTime.Now.ToString("0:MM dd yy H:mm:ss zzz");
            ScheduleAppointment appt = new ScheduleAppointment();
            var randomTest = new Random();
            appt.Sequence = sequence;
            appt.BrokerId = availableBrokerIds[randomTest.Next(availableBrokerIds.Length)];
            appt.CommandId = Guid.NewGuid().ToString();
            appt.ScheduleId = Guid.NewGuid().ToString();
            appt.Title = uniqueText;
            appt.TypeOfCommand = CommandType.ScheduleAppointment;
            DateTime[] range = FetchRandomSlot();

            appt.Start = range[0];
            appt.End = range[1];
            appt.CommandInitiationTimeUtc = DateTime.UtcNow;

            return appt;
        }

        private static DateTime[] FetchRandomSlot()
        {
            // Total span is 10 hours
            TimeSpan timeSpan = new TimeSpan(10, 0, 0);

            // Fetch any random slot (30 minutes duration) in between these 10 hours
            var randomTest = new Random();
            // In 10 hour window total number of consequtive slots (10 hours/ 30 minutes = 20 slots).
            // Fetch any random slot position between 0 and 19 
            int slotPosition = randomTest.Next(0, 19);

            // Start time = 10 AM in morning
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0);
            startDate = startDate.AddMinutes(slotPosition * 30);
            // End time 
            DateTime endDate = startDate.AddMinutes(30);

            return new DateTime[2] { startDate, endDate };
        }
    }
}
