using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureADAuthenticationUtilities.DownStreamApiCall
{
    public static class AuthHandlerExtensions
    {
        public static IApplicationBuilder UseDownstreamApiAuthHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DownstreamApiAuthHandler>();
        }
    }
}
