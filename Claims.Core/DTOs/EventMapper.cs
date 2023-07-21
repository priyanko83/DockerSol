using Claims.Core.Commands;
using Claims.Core.DomainEvents;
using CQRSFramework;
using System.Collections.Generic;

namespace Claims.Core.DTOs
{
    public static class EventMapper
    {
        
        public static DeclarationCreated TransformIntoDomainEvent(CreateNewDeclaration command, long version)
        {
            var newDeclCreated = new DomainEvents.DeclarationCreated(command.Declaration.Title, command.Declaration.DeclarationId, version);

            if (command.Declaration != null && command.Declaration.Incidents != null)
            {
                List<DomainEvents.NewIncidentAdded> events = new List<DomainEvents.NewIncidentAdded>();
                foreach (IncidentDTO incDTO in command.Declaration.Incidents)
                {
                    events.Add(new NewIncidentAdded(incDTO.DeclarationId, incDTO.IncidentId, incDTO.Description, incDTO.DateOfAccident, incDTO.ClaimAmount));
                }
                newDeclCreated.Events = events.ToArray();
            }

            return newDeclCreated;
        }

        public static DeclarationUpdated TransformIntoDomainEvent(UpdateDeclaration command, long version)
        {
            var declUpdated = new DomainEvents.DeclarationUpdated(command.Declaration.Title, command.Declaration.DeclarationId, version);

            if (command.Declaration != null && command.Declaration.Incidents != null)
            {
                List<DomainEvent> events = new List<DomainEvent>();
                foreach (IncidentDTO incDTO in command.Declaration.Incidents)
                {
                    switch(incDTO.TypeOfChange)
                    {
                        case TypeOfModification.Create:
                            events.Add(new NewIncidentAdded(command.Declaration.DeclarationId, incDTO.IncidentId, incDTO.Description, incDTO.DateOfAccident, incDTO.ClaimAmount));
                            break;
                        case TypeOfModification.Edit:
                            events.Add(new IncidentUpdated(command.Declaration.DeclarationId, incDTO.IncidentId, incDTO.Description, incDTO.DateOfAccident, incDTO.ClaimAmount));
                            break;
                        case TypeOfModification.Delete:
                            events.Add(new IncidentDeleted(command.Declaration.DeclarationId, incDTO.IncidentId));
                            break;
                        default:
                            break;
                    }                        
                }
                 
                declUpdated.Events = events.ToArray();
            }

            return declUpdated;
        }
    }
}
