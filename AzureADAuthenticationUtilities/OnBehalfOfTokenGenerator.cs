using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;

namespace AzureADAuthenticationUtilities
{
    public static class OnBehalfOfTokenGenerator
    {
        
        public static async Task<AuthenticationResult> FetchToken(IHttpContextAccessor httpContextAccessor,
            AzureAdOptions config)
        {
            TableStorageWriter writer = new TableStorageWriter();
            try
            {
                
                writer.Write("FetchToken => " + JsonConvert.SerializeObject(config));

                var httpContext = httpContextAccessor.HttpContext;

                //Get the access token used to call this API
                string token = await httpContext.GetTokenAsync("access_token");

                //We are passing an *assertion* to Azure AD about the current user
                //Here we specify that assertion's type, that is a JWT Bearer token
                string assertionType = "urn:ietf:params:oauth:grant-type:jwt-bearer";

                var claimUpn = httpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("preferred_username"));
                string userName = claimUpn?.Value;

                //User name is needed here only for ADAL, it is not passed to AAD
                //ADAL uses it to find a token in the cache if available
                //var user = _httpContext.User;
                //string userName = user.FindFirstValue(ClaimTypes.Upn) ?? user.FindFirstValue(ClaimTypes.Email);

                var userAssertion = new UserAssertion(token, assertionType, userName);

                //Construct the token cache
                //var cache = new DistributedTokenCache(user, _distributedCache, _loggerFactory, _dataProtectionProvider);

                var authContext = new AuthenticationContext(string.Format(config.Instance + "{0}", config.TenantId));
                var clientCredential = new ClientCredential(config.ClientId, config.ClientSecret);
                //Acquire access token
                return await authContext.AcquireTokenAsync(config.DownstreamApiResource, clientCredential, userAssertion);
            }
            catch(Exception ex)
            {
                writer.Write("FetchToken Exception: " + ex.Message);

                if(ex.InnerException != null)
                {
                    writer.Write("FetchToken Exception: " + ex.InnerException.Message);
                }
                throw;
            }
            
        }
    }
}
