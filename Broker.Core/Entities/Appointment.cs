using Brokers.Core.ValueObjects;
using CQRSFramework;
using System;

namespace Brokers.Core.Entities
{
    public class Appointment : Entity<Guid>
    {
        public string Title { get; private set; }
        public string BrokerId { get; private set; }
        public DateTimeRange DateRange { get; private set; }
        public string ScheduleId { get; private set; }
        public Appointment(Guid id) : base(id)
        {

        }
    }
}
