using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MVCWebApplicationExample.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MVCWebApplicationExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this._logger = logger;
        }

        public async Task<IActionResult> IndexAsync([FromServices]IConfiguration configuration)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                //var x = await this.HttpContext.AuthenticateAsync();

                //var accessToken = await this.HttpContext.GetTokenAsync("access_token");
                //var client = new System.Net.Http.HttpClient();
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                //this.ViewData["ApiClaims"] = //Newtonsoft.Json.JsonConvert.SerializeObject( 
                //                             //Newtonsoft.Json.JsonConvert.DeserializeObject( 
                //                                    await client.GetStringAsync($"{this.Request.Scheme}://{this.Request.Host.Value}/Home/Identity");//, 
                //                                    //new Newtonsoft.Json.JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.Indented }
                //                                //)
                //                            //);
            }


            return this.View();
        }

        [Authorize("ApiScope")]
        public IActionResult Identity() => new JsonResult(from c in this.User.Claims select new { c.Type, c.Value, c.Properties });


        [Authorize("profile")]
        public IActionResult RoleProfile()
        {
            this.ViewData["Title"] = "Role: profile";
            return this.View("Role");
        }

        [Authorize("email")]
        public IActionResult RoleEmail()
        {
            this.ViewData["Title"] = "Role: email";
            return this.View("Role");
        }

        [Authorize("email", Roles = "admin")]
        public IActionResult RoleAdmin()
        {
            this.ViewData["Title"] = "Role: admin";
            return this.View("Role");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
