using CQRSFramework;
using System;
using System.Collections.Generic;

namespace Claims.Core.DTOs
{
    [Serializable]
    public class DeclarationDTO
    {
        public string DeclarationId { get; set; }
        public string Title { get; set; }
        public IncidentDTO[] Incidents { get; set; }
    }
}
