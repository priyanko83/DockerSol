using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSFramework.AbstractClasses
{
    public class AggregateRoot<TId> : Entity<TId>
    {
        private readonly Dictionary<Type, Action<DomainEvent, bool>> handlers = new Dictionary<Type, Action<DomainEvent, bool>>();
        private DomainEvent eventToCommit;
        private long LatestVersionNumber = 0;
        
        protected AggregateRoot(TId id) : base(id)
        {            
        }

        /// <summary>
        /// The root parent event object which would have child events (if any) embedded inside it
        /// </summary>
        public DomainEvent EventToCommit
        {
            get { return eventToCommit; }
        }

        /// <summary>
        /// Configures a handler for an event. 
        /// </summary>
        protected void Handles<TEvent>(Action<TEvent, bool> handler)
            where TEvent : DomainEvent
        {
            this.handlers.Add(typeof(TEvent), (@event, newEvent) => handler((TEvent)@event, newEvent));
        }

        /// <summary>
        /// Gets the entity's version. As the entity is being updated and events being generated, the version is incremented.
        /// </summary>
        public long Version
        {
            get { return this.LatestVersionNumber; }
            protected set { this.LatestVersionNumber = value; }
        }


        protected void LoadFrom(IEnumerable<DomainEvent> pastEvents)
        {            
            foreach (var e in pastEvents)
            {
                ApplyChange(e, false, null);                
            }
        }

        protected void ApplyChange(DomainEvent e, bool newEvent, DomainEvent parentEvent)
        {
            this.handlers[e.GetType()].DynamicInvoke(e, newEvent);

            if (parentEvent == null)
            {
                // This means current event (e) is the parent event. 
                this.LatestVersionNumber = e.Version;
            }
            

            if (newEvent)
            {
                if(parentEvent == null)
                {
                    // This means current event (e) is the parent event. So, assign it to EventToCommit
                    this.eventToCommit = e;
                }             
            }            
        }
    }
}
