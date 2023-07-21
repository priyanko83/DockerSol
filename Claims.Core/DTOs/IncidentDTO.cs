using Claims.Core.Entities;
using CQRSFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Claims.Core.DTOs
{
    [Serializable]
    public class IncidentDTO
    {
        public IncidentDTO()
        {

        }

        public IncidentDTO(string declarationId, string incidentId, string desc, DateTime dateOfAcident, double claimAmount, TypeOfModification typeOfChange)
        {
            this.DeclarationId = declarationId;
            this.IncidentId = incidentId;
            this.Description = desc;
            this.DateOfAccident = dateOfAcident;
            this.ClaimAmount = claimAmount;
            this.TypeOfChange = typeOfChange;
        }

        public string DeclarationId { get; set; }
        public string IncidentId { get; set; }        
        public string Description { get; set; }
        public DateTime DateOfAccident { get; set; }
        public TypeOfModification TypeOfChange { get; set; }
        public double ClaimAmount { get; set; }
    }
}
