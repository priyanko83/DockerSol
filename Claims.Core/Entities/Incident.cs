using Claims.Core.ValueObjects;
using CQRSFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Claims.Core.Entities
{
    public class Incident : Entity<Guid>
    {        
        public string Description { get; private set; }
        public DateTime DateOfAccident { get; private set; }
        public List<Damage> IncidentBatch { get; private set; }
        public double ClaimAmount { get; set; }

        /// <summary>
        /// Put more parameters / other valueobjects (such as Coverage/Premium etc) 
        /// that assist in calculation of re-imbursible claim amount
        /// </summary>
        /// <param name="claimAmount"></param>
        /// <param name="natureOfAccident"></param>
        /// <param name="dateOfAccident"></param>
        public Incident(Guid id, string desc, DateTime dateOfAccident, double claimAmount,  List<Damage> IncidentBatch) : base(id)
        {            
            this.IncidentBatch = IncidentBatch;
            this.Description = desc;
            this.DateOfAccident = dateOfAccident;
            this.ClaimAmount = claimAmount;
        }

        /// <summary>
        /// Complex calculation logic goes here based on this ValueObjects properties
        /// </summary>
        /// <returns></returns>
        public double CalculateReimbursibleClaimAmount()
        {
            return new Random().Next(0, 200);
        }
    }
}
