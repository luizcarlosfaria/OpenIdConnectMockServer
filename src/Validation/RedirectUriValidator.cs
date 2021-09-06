using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace OpenIdConnectMockServer.Validation
{
    internal class RedirectUriValidator : IRedirectUriValidator
    {
        protected bool Validate(string requestedUri, ICollection<string> allowedUris)
        {
            bool returnValue = allowedUris.Any(allowedUri => Regex.Match(requestedUri, Regex.Escape(allowedUri).Replace("\\*", "[a-zA-Z0-9.]+?")).Success);

            //TODO: bypass redirectUrl validation
            returnValue = true;

            return returnValue;
        }

        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client) => Task.FromResult(this.Validate(requestedUri, client.PostLogoutRedirectUris));

        public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client) => Task.FromResult(this.Validate(requestedUri, client.RedirectUris));

    }
}
