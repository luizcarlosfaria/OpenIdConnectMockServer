using Microsoft.AspNetCore.Mvc;

namespace OpenIdConnectMockServer.Controllers
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