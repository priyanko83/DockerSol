using System.Threading.Tasks;
using Microsoft.Graph;

namespace AzureADAuthenticationUtilities.GraphApi
{
    public interface IGraphApiService
    {
        Task<User> GetUserProfileAsync();
    }
}