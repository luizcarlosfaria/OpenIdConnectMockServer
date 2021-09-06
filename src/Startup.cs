using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OpenIdConnectMockServer.Helpers;
using OpenIdConnectMockServer.Services;
using OpenIdConnectMockServer.Validation;
using OpenIdConnectMockServer.JsonConverters;
using Newtonsoft.Json.Serialization;
using OpenIdConnectMockServer.Middlewares;
using IdentityServer4.Hosting;
using System.IO;
using System;
using Microsoft.Extensions.Configuration;

namespace OpenIdConnectMockServer
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        private readonly ConfigReporitory configReporitory;

        //System.Security.Claims.ClaimTypes
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.configReporitory = new ConfigReporitory(configuration);
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson(options => {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Converters.Add(new ClaimJsonConverter());
            });

            services.AddIdentityServer(options =>
                    {
                        var configuredOptions = this.configReporitory.GetServerOptions();
                        MergeHelper.Merge(configuredOptions, options);
                    })
                    //.AddDeveloperSigningCredential(filename: Path.Combine(Environment.CurrentDirectory,  "keys", "tempkey.rsa"))
                    .AddDeveloperSigningCredential()
                    .AddInMemoryIdentityResources(this.configReporitory.GetIdentityResources())
                    .AddInMemoryApiResources(this.configReporitory.GetApiResources())
                    .AddInMemoryApiScopes(this.configReporitory.GetApiScopes())
                    .AddInMemoryClients(this.configReporitory.GetClients())
                    .AddTestUsers(this.configReporitory.GetUsers())
                    .AddRedirectUriValidator<RedirectUriValidator>()
                    .AddProfileService<ProfileService>()
                    .AddCorsPolicyService<CorsPolicyService>();

            var aspNetServicesOptions = this.configReporitory.GetAspNetServicesOptions();
            AspNetServicesHelper.ConfigureAspNetServices(services, aspNetServicesOptions);

            this.configReporitory.ConfigureAccountOptions();

            services.AddRouting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            var aspNetServicesOptions = this.configReporitory.GetAspNetServicesOptions();
            AspNetServicesHelper.UseAspNetServices(app, aspNetServicesOptions);

            app.UseIdentityServer();

            var basePath = this.configReporitory.GetAspNetServicesOptions().BasePath;
            if (!string.IsNullOrEmpty(basePath))
            {
                app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments(basePath), appBuilder => {
                    appBuilder.UseMiddleware<BasePathMiddleware>();
                    appBuilder.UseMiddleware<IdentityServerMiddleware>();
                });
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
