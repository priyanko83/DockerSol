namespace CQRSFramework.Commands.Brokers
{
    public class CancelAppointment : ApplicationCommand
    {
        public string AppointmentId { get; set; }

        public CancelAppointment() : base(CommandType.CancelApoointment)
        {
            
        }

    }
}
