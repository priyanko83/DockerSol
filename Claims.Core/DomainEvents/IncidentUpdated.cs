using CQRSFramework;
using System;

namespace Claims.Core.DomainEvents
{
    public class IncidentUpdated : DomainEvent
    {
        public string DeclarationId { get; set; }
        public string IncidentId { get; set; }
        public string Description { get; set; }
        public DateTime DateOfAccident { get; set; }
        public double ClaimAmount { get; set; }

        public IncidentUpdated(string declId, string incId, string desc, DateTime dateOfAccident, double claimAmount) : base(DomainEventType.IncidentUpdated, -1)
        {
            this.DeclarationId = declId;
            this.DateOfAccident = dateOfAccident;
            this.IncidentId = incId;
            this.Description = desc;
            this.ClaimAmount = claimAmount;
        }
    }
}
