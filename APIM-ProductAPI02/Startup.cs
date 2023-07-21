using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureADAuthenticationUtilities.GraphApi;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using ProductAPI02;

namespace ProductAPI02
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.AddControllers();
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddAzureAdBearer(options => Configuration.Bind("AzureAd", options));
            services.AddMvc();

            var aiOptions = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions();
            aiOptions.EnableDependencyTrackingTelemetryModule = true;
            aiOptions.EnableRequestTrackingTelemetryModule = false;

            services.AddApplicationInsightsTelemetry(aiOptions);

            //Important: ProductAPI02TelemetryInitializer enables to set user info in custom events tracking so that funnels could be leveraged
            services.AddSingleton<ITelemetryInitializer, ProductAPI02TelemetryInitializer>();

            services.AddSingleton<IGraphApiService, GraphApiService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthenticationProvider, OnBehalfOfMsGraphAuthenticationProvider>();

            SetupPolicyBasedAuthorizationRules(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            app.UseSwagger();
        }

        private void SetupPolicyBasedAuthorizationRules(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                       "ReadUsingDelegatedPermission",
                       policyBuilder => policyBuilder.RequireAssertion(context => {
                           var claim = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/scope");
                           if (claim == null) { return false; }
                           return claim.Value.Split(' ').Contains("User.View");
                       }));

                options.AddPolicy(
                       "WriteUsingDelegatedPermission",
                       policyBuilder => policyBuilder.RequireAssertion(context => {
                           var claim = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/scope");
                           if (claim == null) { return false; }
                           return claim.Value.Split(' ').Contains("User.Write");
                       }));


                options.AddPolicy("ReadUsingApplicationPermission",
                    policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Product02.ViewAsApp"));

                options.AddPolicy("WriteUsingApplicationPermission",
                    policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Product02.WriteAsApp"));

                options.AddPolicy("ReadUsingViewerSecurityGroup",
                    policy => policy.RequireClaim("groups", "ff4d4d94-6502-410c-b595-8ac581de0dd3", "04de19d0-0da1-4779-a5f0-833a9c293463"));

                options.AddPolicy("WriteUsingContributorSecurityGroup",
                    policy => policy.RequireClaim("groups", "04de19d0-0da1-4779-a5f0-833a9c293463"));
            });
        }
    }
}
