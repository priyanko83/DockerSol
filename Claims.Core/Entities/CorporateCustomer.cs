using Claims.Core.DTOs;
using Claims.Core.DomainEvents;
using Claims.Core.ValueObjects;
using CQRSFramework;
using CQRSFramework.AbstractClasses;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Claims.Core.Entities
{
    public class CorporateCustomer : AggregateRoot<Guid>
    {
        #region Properties        
        public string Name { get; private set; }
        public List<Declaration> AllCorporateDeclarations { get; private set; }
        public double MaxClaimAmount { get; private set; }
        public List<string> EventLog { get; private set; }
        #endregion

        #region Constructors
        public CorporateCustomer(Guid id):base(id)
        {
            this.AllCorporateDeclarations = new List<Declaration>();
            base.Handles<DeclarationCreated>(this.OnDeclarationCreated);
            base.Handles<DeclarationUpdated>(this.OnDeclarationUpdated);
            base.Handles<NewIncidentAdded>(this.OnIncidentAdded);
            base.Handles<IncidentUpdated>(this.OnIncidentUpdated);
            base.Handles<IncidentDeleted>(this.OnIncidentDeleted);
            this.EventLog = new List<string>();
            this.MaxClaimAmount = 900;
        }

        public CorporateCustomer(Guid id, IEnumerable<DomainEvent> history)
            : this(id)
        {
            this.LoadFrom(history);
        }
        #endregion

        #region public methods
        public void CreateDeclaration(DeclarationCreated e)
        {            
            this.ApplyChange(e, true, null);
        }

        public void UpdateDeclaration(DeclarationUpdated e)
        {
            this.ApplyChange(e, true, null);
        }
        #endregion

        #region Domain Events
       

        private void OnDeclarationCreated(DeclarationCreated e, bool newEvent)
        {
            if (this.AllCorporateDeclarations == null)
            {
                this.AllCorporateDeclarations = new List<Declaration>();
            }            

            this.AllCorporateDeclarations.Add(new Declaration(new Guid(e.DeclarationId), e.Title));
            
            foreach (DomainEvent @event in e.Events)
            {
                this.ApplyChange(@event, newEvent, e);
            }

            Declaration decl = FetchDeclaration(e.DeclarationId);
            decl.CalculateReimbursibleClaimAmount();            
        }

        private void OnDeclarationUpdated(DeclarationUpdated e, bool newEvent)
        {            
            Declaration decl = FetchDeclaration(e.DeclarationId);
            decl.UpdateDeclarationMetadata(e);
            SetDeclaration(decl);
            
            foreach(DomainEvent @event in e.Events)
            {
                this.ApplyChange(@event, newEvent, e);
            }

            decl = FetchDeclaration(e.DeclarationId);
            decl.CalculateReimbursibleClaimAmount();            
        }

        private void OnIncidentAdded(NewIncidentAdded e, bool newEvent)
        {
            Declaration decl = FetchDeclaration(e.DeclarationId);

            decl.AddIncident(new Incident(Guid.Parse(e.IncidentId), e.Description, e.DateOfAccident, e.ClaimAmount, null));
            decl.CalculateReimbursibleClaimAmount();

            //if (newEvent)
            //    this.EventLog.Add(string.Format("{0}, is created. Incident Claim Value={1}.",
            //    e.Description, e.ClaimAmount));

            SetDeclaration(decl);
        }

        private void OnIncidentUpdated(IncidentUpdated e, bool newEvent)
        {
            Declaration decl = FetchDeclaration(e.DeclarationId);

            decl.UpdateIncident(new Incident(Guid.Parse(e.IncidentId), e.Description, e.DateOfAccident, e.ClaimAmount, null));
            decl.CalculateReimbursibleClaimAmount();

            //if (newEvent)
            //    this.EventLog.Add(string.Format("{0}, is updated. Incident Claim Value={1}.",
            //        e.Description, e.ClaimAmount));

            SetDeclaration(decl);
        }

        private void OnIncidentDeleted(IncidentDeleted e, bool newEvent)
        {
            Declaration decl = FetchDeclaration(e.DeclarationId);

            decl.DeleteIncident(Guid.Parse(e.IncidentId));
            decl.CalculateReimbursibleClaimAmount();

            //if (newEvent)
            //    this.EventLog.Add(string.Format("Incident {0} is deleted.", e.Id.ToString()));

            SetDeclaration(decl);
        }

        private Declaration FetchDeclaration(string declarationId)
        {
            Guid declId = new Guid(declarationId);
            Declaration decl = this.AllCorporateDeclarations.Find(d => d.Id == declId);

            if (decl == null)
                throw new Exception("Declaration not found");

            return decl;
        }

        private void SetDeclaration(Declaration decl)
        {            
            int index = this.AllCorporateDeclarations.FindIndex(d => d.Id == decl.Id);

            this.AllCorporateDeclarations[index] = decl;
        }
        #endregion

        #region validate aggregate
        public bool IsValidAggregate()
        {
            var totalClaimAmountForCustomer = this.AllCorporateDeclarations.Sum(item => item.ClaimAmount);
            this.EventLog.Add("Adding claim amount for the last declaration = " + this.AllCorporateDeclarations.Last().ClaimAmount);
            this.EventLog.Add("Total claim amount for the customer = " + totalClaimAmountForCustomer);
            return totalClaimAmountForCustomer < 500;
        }
        #endregion
    }
}
