using CQRSFramework;

namespace Claims.Core.DomainEvents
{

    public class DeclarationCreated : DomainEvent
    {
        public string Title { get; set; }
        public string DeclarationId { get; set; }

        public DeclarationCreated(string title, string declId, long version) : base(DomainEventType.DeclarationCreated, version)
        {
            this.Title = title;
            this.DeclarationId = declId;
        }    
    }
}
