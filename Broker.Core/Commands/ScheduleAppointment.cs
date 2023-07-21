using System;

namespace CQRSFramework.Commands.Brokers
{
    [Serializable]
    public class ScheduleAppointment : ApplicationCommand
    {
        public ScheduleAppointment() : base(CommandType.ScheduleAppointment)
        {

        }
        // This is a data transfer object. write getter setters here
        public string Title { get; set; }
        public string BrokerId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string ScheduleId { get; set; }
    }
}
