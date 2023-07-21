using System;

namespace Microsoft.AspNetCore.Authentication
{
    [Serializable]
    public class AzureAdOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string DownstreamApiResource { get; set; }
        public string Instance { get; set; }
        public string Domain { get; set; }
        public string TenantId { get; set; }
        public string CustomerApiBaseUrl { get; set; }
    }
}
