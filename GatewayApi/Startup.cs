using AzureADAuthenticationUtilities.DownStreamApiCall;
using AzureADAuthenticationUtilities.GraphApi;
using GatewayApi.SignalR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.IdentityModel.Logging;

namespace GatewayApi
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
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddAzureAdBearer(options => Configuration.Bind("AzureAd", options));

            services.AddSignalR();

            services.AddMvc();
            IdentityModelEventSource.ShowPII = true;
            services.Configure<EnvironmentConfig>(Configuration);
            services.AddSingleton<IGraphApiService, GraphApiService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthenticationProvider, OnBehalfOfMsGraphAuthenticationProvider>();
            services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(corsPolicyBuilder =>
              corsPolicyBuilder.WithOrigins("http://localhost:4200", "https://staticwebsite83.z5.web.core.windows.net")
              .AllowAnyHeader()
            .WithMethods("GET", "POST")
            .AllowCredentials()
            );

            
            app.UseAuthentication();
            app.UseSignalR(routes =>
            {
                routes.MapHub<NotifyHub>("/notify");
            });
            app.UseMvc();
        }
    }


    public class EnvironmentConfig
    {
        public string CustomerApiUrl { get; set; }
    }
}
