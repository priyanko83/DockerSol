using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI02
{
    public class ProductAPI02TelemetryInitializer : ITelemetryInitializer
    {
        IHttpContextAccessor _httpContextAccessor;
        public ProductAPI02TelemetryInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var ctx = _httpContextAccessor.HttpContext;

            // If telemetry initializer is called as part of request execution and not from some async thread
            if (ctx != null)
            {
                // Set the user id on the Application Insights telemetry item.
                telemetry.Context.User.Id = 
                    ctx.User.Claims.FirstOrDefault(c => c.Type.Contains("preferred_username")).Value;

                // Set the session id on the Application Insights telemetry item.
                // telemetry.Context.Session.Id = "";
            }
        }
    }
}
