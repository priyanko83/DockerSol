using CQRSFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Brokers.Core.Entities
{
    public class BrokerSchedule : Entity<Guid>
    {        
        public IEnumerable<Appointment> Appointments { get; private set; }
        
        public BrokerSchedule(Guid id, List<Appointment> appointments) : base(id)
        {
            this.Appointments = appointments.AsEnumerable();
        }
    }
}
