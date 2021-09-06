using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.Http;
using Microsoft.IdentityModel.Tokens;

namespace MVCWebApplicationExample
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;

            })
            .AddCookie(options =>
            {
                options.LoginPath = new PathString("/perfil/entrar");
                options.LogoutPath = new PathString("/perfil/sair");
                options.AccessDeniedPath = new PathString("/perfil/acesso-negado");
            })
            .AddJwtBearer("Bearer", options =>
            {
                this.Configuration.Bind("bearer", options);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };

            })
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                this.Configuration.Bind("oidc", options);
                
                options.ClaimActions.MapAll();

                //TODO: Falha de Segurança para uso em localhost
                options.BackchannelHttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role,
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnRemoteFailure = context =>
                    {
                        context.Response.Redirect("/");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    },
                    OnRedirectToIdentityProvider = context =>
                    {
                        //context.ProtocolMessage.RedirectUri = $"{this.Configuration.GetValue<string>("applicationUrl")}/signin-oidc";
                        return Task.CompletedTask;
                    },
                };

            });

            services.AddAuthorization(options =>
            {

                options.AddPolicy("email", new AuthorizationPolicyBuilder(OpenIdConnectDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .RequireClaim("email")
                        .Build());

                options.AddPolicy("profile", new AuthorizationPolicyBuilder(OpenIdConnectDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .RequireClaim("email")
                    .RequireClaim("email_verified", "true")
                    .Build());

                options.AddPolicy("admin", new AuthorizationPolicyBuilder(OpenIdConnectDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .RequireClaim("email")
                    .RequireClaim("email_verified", "true")
                    .RequireClaim(ClaimTypes.Role, "admin")
                    .Build());

                //options.AddPolicy("admin", options => options.RequireClaim(ClaimTypes.Role, "admin"));

                options.AddPolicy("ApiScope", new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build());

            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
         

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }

    }
}
