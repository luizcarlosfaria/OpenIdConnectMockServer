using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenIdConnectMockServer
{
    public static class Extensions
    {

        public static IdentityResource AddUserClaim(this IdentityResource identityResource, string userClaim)
        {
            identityResource.UserClaims.Add(userClaim);
            return identityResource;
        }
    }
}
