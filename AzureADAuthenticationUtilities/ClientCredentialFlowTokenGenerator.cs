using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Identity.Client;

namespace AzureADAuthenticationUtilities
{
    public static class ClientCredentialFlowTokenGenerator
    {
        //Ref: https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Client-credential-flows
        public static async Task<AuthenticationResult> FetchToken(AzureAdOptions config)
        {
            var app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
           .WithAuthority(AzureCloudInstance.AzurePublic, config.TenantId)
           .WithClientSecret(config.ClientSecret)
           .Build();
            string[] scopes = new string[] { config.DownstreamApiResource + "/.default" };

            AuthenticationResult result = await app.AcquireTokenForClient(scopes)
                                  .ExecuteAsync();
           
            return result;
        }
    }
}
