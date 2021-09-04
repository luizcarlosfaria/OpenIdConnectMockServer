using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Newtonsoft.Json;
using OpenIdConnectServer.JsonConverters;
using OpenIdConnectServer.Helpers;
using Microsoft.Extensions.Configuration;

namespace OpenIdConnectServer
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
        

        public AspNetServicesOptions GetAspNetServicesOptions() {
            string aspNetServicesOptionsStr = Environment.GetEnvironmentVariable("ASPNET_SERVICES_OPTIONS_INLINE");
            if (string.IsNullOrWhiteSpace(aspNetServicesOptionsStr))
            {
                var aspNetServicesOptionsPath = Environment.GetEnvironmentVariable("ASPNET_SERVICES_OPTIONS_PATH");
                if (string.IsNullOrWhiteSpace(aspNetServicesOptionsPath))
                {
                    return new AspNetServicesOptions();
                }
                aspNetServicesOptionsStr = File.ReadAllText(aspNetServicesOptionsPath);
            }
            var aspNetServicesOptions = JsonConvert.DeserializeObject<AspNetServicesOptions>(aspNetServicesOptionsStr);
            return aspNetServicesOptions;
        }

        public IdentityServerOptions GetServerOptions()
        {
            string serverOptionsStr = Environment.GetEnvironmentVariable("SERVER_OPTIONS_INLINE");
            if (string.IsNullOrWhiteSpace(serverOptionsStr))
            {
                var serverOptionsFilePath = Environment.GetEnvironmentVariable("SERVER_OPTIONS_PATH");
                if (string.IsNullOrWhiteSpace(serverOptionsFilePath))
                {
                    return new IdentityServerOptions();
                }
                serverOptionsStr = File.ReadAllText(serverOptionsFilePath);
            }
            var serverOptions = JsonConvert.DeserializeObject<IdentityServerOptions>(serverOptionsStr);
            return serverOptions;
        }

        public void ConfigureAccountOptions()
        {
            string accountOptionsStr = Environment.GetEnvironmentVariable("ACCOUNT_OPTIONS_INLINE");
            if (string.IsNullOrWhiteSpace(accountOptionsStr))
            {
                var accountOptionsFilePath = Environment.GetEnvironmentVariable("ACCOUNT_OPTIONS_PATH");
                if (string.IsNullOrWhiteSpace(accountOptionsFilePath))
                {
                    return;
                }
                accountOptionsStr = File.ReadAllText(accountOptionsFilePath);
            }
            AccountOptionsHelper.ConfigureAccountOptions(accountOptionsStr);
        }

        public IEnumerable<string> GetServerCorsAllowedOrigins()
        {
            string allowedOriginsStr = Environment.GetEnvironmentVariable("SERVER_CORS_ALLOWED_ORIGINS_INLINE");
            if (string.IsNullOrWhiteSpace(allowedOriginsStr))
            {
                var allowedOriginsFilePath = Environment.GetEnvironmentVariable("SERVER_CORS_ALLOWED_ORIGINS_PATH");
                if (string.IsNullOrWhiteSpace(allowedOriginsFilePath))
                {
                    return null;
                }
                allowedOriginsStr = File.ReadAllText(allowedOriginsFilePath);
            }
            var allowedOrigins = JsonConvert.DeserializeObject<IEnumerable<string>>(allowedOriginsStr);
            return allowedOrigins;
        }

        public IEnumerable<ApiScope> GetApiScopes()
        {
            string apiScopesStr = Environment.GetEnvironmentVariable("API_SCOPES_INLINE");
            if (string.IsNullOrWhiteSpace(apiScopesStr))
            {
                var apiScopesFilePath = Environment.GetEnvironmentVariable("API_SCOPES_PATH");
                if (string.IsNullOrWhiteSpace(apiScopesFilePath))
                {
                    return new List<ApiScope>();
                }
                apiScopesStr = File.ReadAllText(apiScopesFilePath);
            }
            var apiScopes = JsonConvert.DeserializeObject<IEnumerable<ApiScope>>(apiScopesStr);
            return apiScopes;
        }

        public IEnumerable<ApiResource> GetApiResources()
        {
            string apiResourcesStr = Environment.GetEnvironmentVariable("API_RESOURCES_INLINE");
            if (string.IsNullOrWhiteSpace(apiResourcesStr))
            {
                var apiResourcesFilePath = Environment.GetEnvironmentVariable("API_RESOURCES_PATH");
                if (string.IsNullOrWhiteSpace(apiResourcesFilePath))
                {
                    return new List<ApiResource>();
                }
                apiResourcesStr = File.ReadAllText(apiResourcesFilePath);
            }
            var apiResources = JsonConvert.DeserializeObject<IEnumerable<ApiResource>>(apiResourcesStr, new SecretJsonConverter());
            return apiResources;
        }

        public IEnumerable<Client> GetClients()
        {
            string configStr = Environment.GetEnvironmentVariable("CLIENTS_CONFIGURATION_INLINE");
            if (string.IsNullOrWhiteSpace(configStr))
            {
                var configFilePath = Environment.GetEnvironmentVariable("CLIENTS_CONFIGURATION_PATH");
                if (string.IsNullOrWhiteSpace(configFilePath))
                {
                    throw new ArgumentNullException("You must set either CLIENTS_CONFIGURATION_INLINE or CLIENTS_CONFIGURATION_PATH env variable");
                }
                configStr = File.ReadAllText(configFilePath);
            }
            var configClients = JsonConvert.DeserializeObject<IEnumerable<Client>>(configStr, new SecretJsonConverter(), new ClaimJsonConverter());
            return configClients;
        }

        public IEnumerable<IdentityResource> GetIdentityResources()
        {
            var standardResources = new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource(
                    name: "roles",
                    userClaims: new[] { "resource_access" },
                    displayName: "Your profile data")
                };
            return standardResources.Union(this.GetCustomIdentityResources());
        }

        public List<TestUser> GetUsers()
        {
            string configStr = Environment.GetEnvironmentVariable("USERS_CONFIGURATION_INLINE");
            if (string.IsNullOrWhiteSpace(configStr))
            {
                var configFilePath = Environment.GetEnvironmentVariable("USERS_CONFIGURATION_PATH");
                if (string.IsNullOrWhiteSpace(configFilePath))
                {
                    return new List<TestUser>();
                }
                configStr = File.ReadAllText(configFilePath);
            }
            var configUsers = JsonConvert.DeserializeObject<List<TestUser>>(configStr, new ClaimJsonConverter());
            return configUsers;
        }

        private IEnumerable<IdentityResource> GetCustomIdentityResources()
        {
            string identityResourcesStr = Environment.GetEnvironmentVariable("IDENTITY_RESOURCES_INLINE");
            if (string.IsNullOrWhiteSpace(identityResourcesStr))
            {
                var identityResourcesFilePath = Environment.GetEnvironmentVariable("IDENTITY_RESOURCES_PATH");
                if (string.IsNullOrWhiteSpace(identityResourcesFilePath))
                {
                    return new List<IdentityResource>();
                }
                identityResourcesStr = File.ReadAllText(identityResourcesFilePath);
            }

            var identityResourceConfig = JsonConvert.DeserializeObject<IdentityResourceConfig[]>(identityResourcesStr);
            return identityResourceConfig.Select(c => new IdentityResource(c.Name, c.ClaimTypes));
        }

        private class IdentityResourceConfig
        {
            public string Name { get; set; }
            public IEnumerable<string> ClaimTypes { get; set; }
        }
    }
}
