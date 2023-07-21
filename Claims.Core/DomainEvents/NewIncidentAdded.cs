using CQRSFramework;
using System;

namespace Claims.Core.DomainEvents
{
    public class NewIncidentAdded : DomainEvent
    {
        public string DeclarationId { get; set; }
        public string IncidentId { get; set; }
        public string Description { get; set; }
        public DateTime DateOfAccident { get; set; }
        public double ClaimAmount { get; set; }

        public NewIncidentAdded(string declId, string incId, string desc, DateTime dateOfAccident, double claimAmount) : base(DomainEventType.IncidentCreated, -1)
        {
            this.DeclarationId = declId;
            this.IncidentId = incId;
            this.Description = desc;
            this.DateOfAccident = dateOfAccident;
            this.ClaimAmount = claimAmount;
        }
    }
}
