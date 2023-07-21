using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureADAuthenticationUtilities.DownStreamApiCall
{
    public class DownstreamApiAuthHandler : DelegatingHandler
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string TenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";
        private readonly AzureAdOptions _config;

        public DownstreamApiAuthHandler(RequestDelegate next, IHttpContextAccessor httpContextAccessor, IOptions<AzureAdOptions> azureOptions)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
            _config = azureOptions.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
        {
            
            return await base.SendAsync(request, cancellationToken);
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var result = await OnBehalfOfTokenGenerator.FetchToken(_httpContextAccessor, _config);
                context.Request.Headers["Authorization"] = string.Format("Bearer {0}", result.AccessToken);
            }
            catch(Exception ex) {
                var str = ex.Message;
            }
            await _next.Invoke(context);
        }

    }
}
