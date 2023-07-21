using CQRSFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Claims.Core.DomainEvents
{
    public class DeclarationUpdated : DomainEvent
    {
        public string Title { get; set; }
        public string DeclarationId { get; set; }

        public DeclarationUpdated(string title, string declId, long version) : base(DomainEventType.DeclarationUpdated, version)
        {
            this.Title = title;
            this.DeclarationId = declId;
        }
    }
}
