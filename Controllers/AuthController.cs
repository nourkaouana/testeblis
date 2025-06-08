using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using testbills.Services;
using testbills.Models; // Adapte si nécessaire

namespace testbills.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegistrationRequest request)
        {
            var result = await _userService.RegisterUserAsync(request);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> AuthenticateUser([FromBody] AuthRequest request)
        {
            var authResponse = await _userService.AuthenticateUserAsync(request);
            if (authResponse == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
            return Ok(authResponse);
        }
    }
}
