using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AzureADAuthenticationUtilities.GraphApi;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace ProductAPI02.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class Product02Controller : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IGraphApiService _graphApiService;
        private readonly TelemetryClient _telemetryClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Product02Controller(ILogger<Product02Controller> logger, IGraphApiService graphApiService, TelemetryClient telemetryClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _graphApiService = graphApiService;
            _logger = logger;
            _telemetryClient = telemetryClient;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet]
        [Route("Stage02")]
        public async Task<IEnumerable<string>> Stage02()
        {
            return await FetchClaims();
        }

        #region Delegated Permission
        [HttpGet]
        [Route("ReadUsingDelegatedPermission")]
        [Authorize(Policy = "ReadUsingDelegatedPermission")]
        public async Task<string[]> ReadUsingDelegatedPermission()
        {
            _telemetryClient.TrackEvent("Stage02-ReadUsingDelegatedPermission", new Dictionary<string, string>() { { "Time Of Invocation", DateTime.Now.ToString() } });
            return new string[] { "Successfully invoked ReadUsingDelegatedPermission" };
        }

        [HttpGet]
        [Route("WriteUsingDelegatedPermission")]
        [Authorize(Policy = "WriteUsingDelegatedPermission")]
        public async Task<string[]> WriteUsingDelegatedPermission()
        {
            _telemetryClient.TrackEvent("Stage02-WriteUsingDelegatedPermission", new Dictionary<string, string>() { { "Time Of Invocation", DateTime.Now.ToString() } });
            return new string[] { "Successfully invoked WriteUsingDelegatedPermission" };
        }
        #endregion

        #region Application Permission
        [HttpGet]
        [Route("ReadUsingApplicationPermission")]
        [Authorize(Policy = "ReadUsingApplicationPermission")]
        public async Task<string[]> ReadUsingApplicationPermission()
        {
            _telemetryClient.TrackEvent("Stage02-ReadUsingApplicationPermission", new Dictionary<string, string>() { { "Time Of Invocation", DateTime.Now.ToString() } });
            return new string[] { "Successfully invoked ReadUsingApplicationPermission" };
        }

        [HttpGet]
        [Route("WriteUsingApplicationPermission")]
        [Authorize(Policy = "WriteUsingApplicationPermission")]
        public async Task<string[]> WriteUsingApplicationPermission()
        {
            _telemetryClient.TrackEvent("Stage02-WriteUsingApplicationPermission", new Dictionary<string, string>() { { "Time Of Invocation", DateTime.Now.ToString() } });
            return new string[] { "Successfully invoked WriteUsingApplicationPermission" };
        }
        #endregion

        #region Groups Claim validation (Delegated Permission Calls)
        [HttpGet]
        [Route("ReadUsingViewerSecurityGroup")]
        [Authorize(Policy = "ReadUsingViewerSecurityGroup")]
        public async Task<string[]> ReadUsingViewerSecurityGroup()
        {
            _telemetryClient.TrackEvent("Stage02-ReadUsingViewerSecurityGroup", new Dictionary<string, string>() { { "Time Of Invocation", DateTime.Now.ToString() } });

            return new string[] { "Successfully invoked ReadUsingViewerSecurityGroup" };
        }

        [HttpGet]
        [Route("WriteUsingContributorSecurityGroup")]
        [Authorize(Policy = "WriteUsingContributorSecurityGroup")]
        public async Task<string[]> WriteUsingContributorSecurityGroup()
        {
            _telemetryClient.TrackEvent("Stage02-WriteUsingContributorSecurityGroup", new Dictionary<string, string>() { { "Time Of Invocation", DateTime.Now.ToString() } });
            return new string[] { "Successfully invoked WriteUsingContributorSecurityGroup" };
        }
        #endregion

        #region Reusable utility methods
        private string FetchProperties(Claim claim)
        {
            string dictionaryString = "";
            foreach (KeyValuePair<string, string> keyValues in claim.Properties)
            {
                dictionaryString += keyValues.Key + " : " + keyValues.Value + ", ";
            }

            return dictionaryString;
        }

        private async Task<IEnumerable<string>> FetchClaims()
        {
            List<string> strclaims = new List<string>();

            var claims = _httpContextAccessor.HttpContext.User.Claims.ToList();

            foreach (Claim claim in claims)
            {
                strclaims.Add($"{claim.Type} =====>>>> {claim.Value}");

            }

            if (ClaimsPrincipal.Current != null)
            {
                foreach (Claim claim in ClaimsPrincipal.Current.Claims)
                {
                    strclaims.Add($"Application {claim.Type} =====>>>> {claim.Value}");

                }
            }

            strclaims.Add($"Product02 - Stage 02 - Graph Api called: {await InvokeGraphAPI() } at {DateTime.Now}");

            return strclaims;
        }

        private async Task<string> InvokeGraphAPI()
        {
            string output = "";
            try
            {
                var userProfile = await _graphApiService.GetUserProfileAsync();
                output = userProfile.UserPrincipalName;

            }
            catch (Exception ex)
            {
                output = ex.Message;
                if (ex.InnerException != null)
                {
                    output += " - " + ex.InnerException.Message;
                }
            }

            return output;
        }

        private void ValidateAppRole(string appRole)
        {
            //
            // The `role` claim tells you what permissions the client application has in the service.
            // In this case, we look for a `role` value of `access_as_application`.
            //
            Claim roleClaim = ClaimsPrincipal.Current.FindFirst("roles");
            if (roleClaim == null || !roleClaim.Value.Split(' ').Contains(appRole))
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    ReasonPhrase = $"The 'roles' claim does not contain '{appRole}' or was not found"
                });
            }
        }

        private void ValidateGroup(string groupName)
        {
            //
            // The `role` claim tells you what permissions the client application has in the service.
            // In this case, we look for a `role` value of `access_as_application`.
            //
            Claim roleClaim = ClaimsPrincipal.Current.FindFirst("groups");
            if (roleClaim == null || !roleClaim.Value.Split(' ').Contains(groupName))
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    ReasonPhrase = $"The 'groups' claim does not contain '{groupName}' or was not found"
                });
            }
        }


       
        #endregion
    }
}