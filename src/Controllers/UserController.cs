using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace OpenIdConnectMockServer.Controllers
{
  [Route("api/v1/user")]
    public class UserController: Controller
    {
        private readonly TestUserStore _usersStore;
        private readonly ILogger Logger;

        public UserController(TestUserStore userStore, ILogger<UserController> logger)
        {
            this._usersStore = userStore;
            this.Logger = logger;
        }

        [HttpGet("{subjectId}")]
        public IActionResult GetUser([FromRoute]string subjectId)
        {
            var user = this._usersStore.FindBySubjectId(subjectId);
            this.Logger.LogDebug("User found: {subjectId}", subjectId);
            return this.Json(user);
        }

        [HttpPost]
        public IActionResult AddUser([FromBody]TestUser user)
        {
            var claims = new List<Claim>(user.Claims);
            claims.Add(new Claim(ClaimTypes.Name, user.Username));
            var newUser = this._usersStore.AutoProvisionUser("Alex", user.SubjectId, new List<Claim>(user.Claims));
            newUser.SubjectId = user.SubjectId;
            newUser.Username = user.Username;
            newUser.Password = user.Password;
            newUser.ProviderName = string.Empty;
            newUser.ProviderSubjectId = string.Empty;

            this.Logger.LogDebug("New user added: {user}", user.SubjectId);

            return this.Json(user.SubjectId);
        }
    }
}
