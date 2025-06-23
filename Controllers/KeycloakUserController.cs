using KeycloakAuthDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeycloakAuthDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
 
    public class KeycloakUserController : ControllerBase
    {
        private readonly KeycloakAdminService _keyService;

        public KeycloakUserController(KeycloakAdminService keyService)
        {
            _keyService = keyService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            if (await _keyService.CreateUserAsync(dto))
                return Ok(new { message = "User created" });
            return BadRequest(new { error = "Failed to create user" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _keyService.GetUsersAsync();
            return Ok(users);
        }
    }

}
