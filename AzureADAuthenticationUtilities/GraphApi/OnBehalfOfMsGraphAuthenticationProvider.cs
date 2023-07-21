using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AzureADAuthenticationUtilities.GraphApi
{
    public class OnBehalfOfMsGraphAuthenticationProvider : IAuthenticationProvider
    {       
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string TenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";
        private readonly AzureAdOptions _config;

        public OnBehalfOfMsGraphAuthenticationProvider(IHttpContextAccessor httpContextAccessor, 
            IOptions<AzureAdOptions> azureOptions)
        {           
            _httpContextAccessor = httpContextAccessor;
            _config = azureOptions.Value;
        }

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            try
            {
                var result = await OnBehalfOfTokenGenerator.FetchToken(_httpContextAccessor, _config);
                //Set the authentication header
                request.Headers.Authorization = new AuthenticationHeaderValue(result.AccessTokenType, result.AccessToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
