using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeycloakAuthDemo.Controllers
{
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult Public() => Ok("🌍 This is a public endpoint.");

        [HttpGet("secure")]
        [Authorize]
        public IActionResult Secure() => Ok("🔒 You are authenticated.");

        [Authorize(Policy = "admin")]

        [HttpGet("admin")]
        public IActionResult Admin()
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return Forbid();
            }

            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(new
            {
                message = "✅ You are authorized",
                claims
            });
        }


        [Authorize]
        [HttpGet("claims")]
        public IActionResult Claims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }



    }
}
