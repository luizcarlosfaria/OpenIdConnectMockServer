using Microsoft.AspNetCore.Mvc;

namespace OpenIdConnectServer.Controllers
{
    public class HealthController : Controller
    {
        [HttpGet("/health")]
        public IActionResult Get()
        {
            return this.Ok();
        }
    }
}