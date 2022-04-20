using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Webservice.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Modarator")]
        public async Task<IActionResult> Modarator()
        {
            var user = HttpContext.User.Claims;
            return this.Ok("User Authorized and Role 'Modarator' Exist");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var user = HttpContext.User.Claims;
            return this.Ok("User Authorized and Role 'Admin' Exist");
        }
    }
}
