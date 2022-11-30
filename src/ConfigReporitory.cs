using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Newtonsoft.Json;
using OpenIdConnectMockServer.JsonConverters;
using OpenIdConnectMockServer.Helpers;
using Microsoft.Extensions.Configuration;

namespace OpenIdConnectMockServer
{
    public class ConfigReporitory
    {
        public static ConfigReporitory Instance { get; private set; }

        private readonly IConfiguration configuration;

        public ConfigReporitory(IConfiguration configuration)
        {
            this.configuration = configuration;
            Instance = this;
        }

        private T GetConfiguration<T>(string key, Func<T> empty, params JsonConverter[] converters)
        {
            string content = this.configuration.GetValue<string>($"{key}_INLINE");
            if (string.IsNullOrWhiteSpace(content))
            {
                var path = this.configuration.GetValue<string>($"{key}_PATH");
                if (string.IsNullOrWhiteSpace(path))
                {
                    return empty();
                }
                content = File.ReadAllText(path);
            }
            var returnValue = JsonConvert.DeserializeObject<T>(content, converters);
            return returnValue;
        }


        public AspNetServicesOptions GetAspNetServicesOptions() => this.GetConfiguration("ASPNET_SERVICES_OPTIONS", () => new AspNetServicesOptions());

        public IdentityServerOptions GetServerOptions() => this.GetConfiguration("SERVER_OPTIONS", () => new IdentityServerOptions());


        public void ConfigureAccountOptions()
        {
            string accountOptionsStr = this.configuration.GetValue<string>("ACCOUNT_OPTIONS_INLINE");
            if (string.IsNullOrWhiteSpace(accountOptionsStr))
            {
                var accountOptionsFilePath = this.configuration.GetValue<string>("ACCOUNT_OPTIONS_PATH");
                if (string.IsNullOrWhiteSpace(accountOptionsFilePath))
                {
                    return;
                }
                accountOptionsStr = File.ReadAllText(accountOptionsFilePath);
            }
            AccountOptionsHelper.ConfigureAccountOptions(accountOptionsStr);
        }

        public IEnumerable<string> GetServerCorsAllowedOrigins() => this.GetConfiguration<IEnumerable<string>>("SERVER_CORS_ALLOWED_ORIGINS", () => null);

        public IEnumerable<ApiScope> GetApiScopes() => this.GetConfiguration("API_SCOPES", () => new List<ApiScope>());

        public IEnumerable<ApiResource> GetApiResources() => this.GetConfiguration<IEnumerable<ApiResource>>("API_RESOURCES", () => new List<ApiResource>(), new SecretJsonConverter());

        public IEnumerable<Client> GetClients() => this.GetConfiguration<IEnumerable<Client>>("CLIENTS_CONFIGURATION", () => throw new ArgumentNullException("You must set either CLIENTS_CONFIGURATION_INLINE or CLIENTS_CONFIGURATION_PATH env variable"), new SecretJsonConverter(), new ClaimJsonConverter());


        public List<TestUser> GetUsers() => this.GetConfiguration("USERS_CONFIGURATION", () => new List<TestUser>(), new ClaimJsonConverter());

        private IEnumerable<IdentityResource> GetCustomIdentityResources() => this.GetConfiguration<IEnumerable<IdentityResourceConfig>>("IDENTITY_RESOURCES", () => new List<IdentityResourceConfig>()).Select(c => new IdentityResource(c.Name, c.ClaimTypes));

        public IEnumerable<IdentityResource> GetIdentityResources()
        {
            var standardResources = new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile().AddUserClaim("name"),
                new IdentityResources.Email().AddUserClaim("email_verified"),
                new IdentityResource(
                    name: "roles",
                    userClaims: new[] { "resource_access", "role", System.Security.Claims.ClaimTypes.Role },
                    displayName: "Your profile data")
                };
            return standardResources.Union(this.GetCustomIdentityResources());
        }

        private class IdentityResourceConfig
        {
            public string Name { get; set; }
            public IEnumerable<string> ClaimTypes { get; set; }
        }
    }
}
