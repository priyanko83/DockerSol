using Brokers.Core.Entities;
using CQRSFramework;

namespace Brokers.Core.Events
{
    public class AppointmentCancelled : DomainEvent
    {
        public Appointment Appointment { get; private set; }
        public AppointmentCancelled(long sequenceId, Appointment appointment) : base(DomainEventType.AppointmentCancelled, sequenceId)
        {
            this.Appointment = appointment;
        }
    }
}
