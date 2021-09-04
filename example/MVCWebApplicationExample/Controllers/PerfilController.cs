using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MVCWebApplicationExample.Controllers
{
    public class PerfilController : Controller
    {
        [Authorize()]
        public IActionResult Index() => this.View();

        [Authorize()]
        public IActionResult entrar() => this.RedirectToAction("Index", "Home");

        [Route("perfil/acesso-negado")]
        public IActionResult AcessoNegado() => this.View();

        public IActionResult Privacidade() => this.View();

        [Authorize()]
        [Route("perfil/defaultSubscriptionData")]
        public IActionResult ObterDadosFormInscricao()
        {
            return this.Json(new
            {
                firstName = this.User.Claims.SingleOrDefault(it => it.Type == System.Security.Claims.ClaimTypes.GivenName)?.Value ?? string.Empty,
                lastName = this.User.Claims.SingleOrDefault(it => it.Type == System.Security.Claims.ClaimTypes.Surname)?.Value ?? string.Empty,
                email = this.User.Claims.SingleOrDefault(it => it.Type == System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty
            });
        }

        public async Task<IActionResult> Sair([FromServices] IConfiguration configuration)
        {
            await this.HttpContext.SignOutAsync("Cookies");

            await this.HttpContext.SignOutAsync("OpenIdConnect");

            string path = UrlEncoder.Default.Encode($"{this.Request.Scheme}://{this.Request.Host.Value}/");

            //Keycloak
            //return this.Redirect($"{configuration.GetValue<string>("oidc:Authority")}/protocol/openid-connect/logout?redirect_uri={path}"); //?=encodedRedirectUri

            //Identity
            return this.Redirect($"{configuration.GetValue<string>("oidc:Authority")}/account/forcelogout?redirect_uri={path}"); //?=encodedRedirectUri

            //return RedirectToAction("Index", "Home");
        }
    }
}
