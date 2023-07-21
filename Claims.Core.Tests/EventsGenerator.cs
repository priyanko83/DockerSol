using Claims.Core.DTOs;
using Claims.Core.DomainEvents;
using Claims.Core.Entities;
using CQRSFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Claims.Core.Tests
{
    public class EventsGenerator
    {
        private static long EventVersion = 0;

        public static void LoadAggregate()
        {
            CorporateCustomer corp = new CorporateCustomer(Guid.NewGuid(), GeneratePastEvents());            
        }
        
        private static List<DomainEvent> GeneratePastEvents()
        {
            List<DomainEvent> pastEvents = new List<DomainEvent>();
            for (int i = 0; i < 5; i++)
            {
                CreateNewDeclaration(i, pastEvents);
            }

            return pastEvents;
        }

        private static List<DomainEvent> GenerateNewEvents()
        {            
            List<DomainEvent> newEvents = new List<DomainEvent>();
            for (int i = 5; i < 6; i++)
            {
                CreateNewDeclaration(i, newEvents);
            }

            return newEvents;
        }

        private static void CreateNewDeclaration(int count, List<DomainEvent> eventsCollection)
        {
            Guid declId = Guid.NewGuid();
            List<Guid> incidentGuids = new List<Guid>();
            for (int i = 0; i < 5; i++)
            {
                incidentGuids.Add(Guid.NewGuid());
            }


            string title = string.Format("Declaration {0}", count + 1);
            EventVersion++;
            DeclarationCreated declCreated = new DeclarationCreated(title, declId.ToString(), EventVersion);
            eventsCollection.Add(declCreated);
            
            for (int i = 0; i < 5; i++)
            {
                eventsCollection.Add(new NewIncidentAdded(declCreated.DeclarationId, incidentGuids[i].ToString(), string.Format("New Incident {0}", i + 1), DateTime.Now.AddDays(-new Random().Next(0, 365)), 0));                
            }
            
            for (int i = 1; i < 4; i++)
            {
                eventsCollection.Add(new IncidentUpdated(declCreated.DeclarationId, incidentGuids[i].ToString(), string.Format("New Incident {0}", i + 1), DateTime.Now.AddDays(-new Random().Next(0, 365)), 0));
            }
        }
    }
}
