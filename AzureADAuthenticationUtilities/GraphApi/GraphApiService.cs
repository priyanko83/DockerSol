using Microsoft.Graph;
using System.Threading.Tasks;

namespace AzureADAuthenticationUtilities.GraphApi
{
    public class GraphApiService : IGraphApiService
    {
        private readonly IAuthenticationProvider _msGraphAuthenticationProvider;

        public GraphApiService(IAuthenticationProvider authenticationProvider)
        {
            _msGraphAuthenticationProvider = authenticationProvider;
        }

        public async Task<User> GetUserProfileAsync()
        {
            var client = new GraphServiceClient(_msGraphAuthenticationProvider);
            return await client.Me.Request().Select("DisplayName,Mail, UserPrincipalName").GetAsync();
        }
    }
}
