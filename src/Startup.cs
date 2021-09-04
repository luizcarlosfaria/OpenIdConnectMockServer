using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OpenIdConnectServer.Helpers;
using OpenIdConnectServer.Services;
using OpenIdConnectServer.Validation;
using OpenIdConnectServer.JsonConverters;
using Newtonsoft.Json.Serialization;
using OpenIdConnectServer.Middlewares;
using IdentityServer4.Hosting;
using System.IO;
using System;

namespace OpenIdConnectServer
{
    public class Startup
    {

        //System.Security.Claims.ClaimTypes

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
                        var configuredOptions = Config.GetServerOptions();
                        MergeHelper.Merge(configuredOptions, options);
                    })
                    //.AddDeveloperSigningCredential(filename: Path.Combine(Environment.CurrentDirectory,  "keys", "tempkey.rsa"))
                    .AddDeveloperSigningCredential()
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryApiResources(Config.GetApiResources())
                    .AddInMemoryApiScopes(Config.GetApiScopes())
                    .AddInMemoryClients(Config.GetClients())
                    .AddTestUsers(Config.GetUsers())
                    .AddRedirectUriValidator<RedirectUriValidator>()
                    .AddProfileService<ProfileService>()
                    .AddCorsPolicyService<CorsPolicyService>();

            var aspNetServicesOptions = Config.GetAspNetServicesOptions();
            AspNetServicesHelper.ConfigureAspNetServices(services, aspNetServicesOptions);

            Config.ConfigureAccountOptions();

            services.AddRouting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            var aspNetServicesOptions = Config.GetAspNetServicesOptions();
            AspNetServicesHelper.UseAspNetServices(app, aspNetServicesOptions);

            app.UseIdentityServer();

            var basePath = Config.GetAspNetServicesOptions().BasePath;
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
