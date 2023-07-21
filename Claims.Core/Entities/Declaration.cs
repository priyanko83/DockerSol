using Claims.Core.DomainEvents;
using CQRSFramework;
using System;
using System.Collections.Generic;

namespace Claims.Core.Entities
{
    public class Declaration : Entity<Guid>
    {
        #region Properties
        public string Title { get; private set; }
        public List<Incident> Incidents { get; private set; }
        public double ClaimAmount { get; private set; }
        #endregion

        #region constructors
        protected Declaration(Guid id) : base(id)
        {
            
        }

        public Declaration(Guid id, string title) : this(id)
        {
            this.Title = title;
        }
        #endregion

        #region public Methods
        public void UpdateDeclarationMetadata(DeclarationUpdated e)
        {
            this.Title = e.Title;            
        }
        public void AddIncident(Incident item)
        {
            if (this.Incidents == null)
            {
                this.Incidents = new List<Incident>();
            }

            this.Incidents.Add(item);
        }

        public void UpdateIncident(Incident item)
        {            
            var index = this.Incidents.FindIndex(d => d.Id == item.Id);

            if (index >= 0)
            {
                this.Incidents[index] = item;
            }
        }

        public void DeleteIncident(Guid id)
        {
            var index = this.Incidents.FindIndex(d => d.Id == id);

            if (index >= 0)
            {
                this.Incidents.RemoveAt(index);
            }
        }

        public void CalculateReimbursibleClaimAmount()
        {
            ClaimAmount = 0;
            foreach(Incident inc in this.Incidents)
            {
                ClaimAmount += inc.ClaimAmount;
            }
        }
        #endregion

    }
}
