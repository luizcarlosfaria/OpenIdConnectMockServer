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

        public IActionResult Index()
        {
            return this.View();
        }

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
