using Brokers.Core.Entities;
using CQRSFramework;

namespace Brokers.Core.Events
{
    public class AppointmentScheduled : DomainEvent
    {
        public Appointment Appointment { get; private set;}
        public AppointmentScheduled(long sequenceId, Appointment appointment) : base(DomainEventType.AppointmentScheduled, sequenceId)
        {
            this.Appointment = appointment;
        }
    }
}
