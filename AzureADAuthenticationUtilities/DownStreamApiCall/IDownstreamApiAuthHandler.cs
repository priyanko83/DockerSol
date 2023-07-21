using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace AzureADAuthenticationUtilities.DownStreamApiCall
{
    public interface IDownstreamApiAuthHandler
    {
        Task SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}
