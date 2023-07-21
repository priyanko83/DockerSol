using CQRSFramework;

namespace Claims.Core.DomainEvents
{
    public class IncidentDeleted : DomainEvent
    {
        public string DeclarationId { get; set; }
        public string IncidentId { get; set; }
        public IncidentDeleted(string declId, string incId) : base(DomainEventType.IncidentDeleted, -1)
        {
            this.DeclarationId = declId;            
            this.IncidentId = incId;
        }
    }
}
