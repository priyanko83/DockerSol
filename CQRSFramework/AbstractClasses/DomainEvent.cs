using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSFramework
{    
    public abstract class DomainEvent
    {
        public Guid Id { get; set; }
        public DateTime DateTimeEventOccurred { get; set; }        
        public long Version { get; set; }
        public DomainEventType TypeOfEvent { get; set; }

        public DomainEvent[] Events { get; set; }

        public DomainEvent()
        {

        }

        public DomainEvent(DomainEventType typeOfEvent, long version)
        {            
            this.Id = Guid.NewGuid();
            this.DateTimeEventOccurred = DateTime.Now;
            this.TypeOfEvent = typeOfEvent;
            this.Version = version;
        }
    }
}
