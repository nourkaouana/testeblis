using testbills.Models;
using JwtRoleAuthentication.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using testbills.Services;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

      //  [HttpPost("register")]
        //[AllowAnonymous]
        //public async Task<IActionResult> RegisterUser([FromBody] RegistrationRequest request)
        //{
          //  var result = await _userService.RegisterUserAsync(request);
            //if (!result.Succeeded)
            //{
              //  return BadRequest(result.Errors);
           // }
           // return Ok(new { message = "User registered successfully" });
       // }

      //  [HttpPost("authenticate")]
        //[AllowAnonymous]
       // public async Task<IActionResult> AuthenticateUser([FromBody] AuthRequest request)
        //{
          //  var authResponse = await _userService.AuthenticateUserAsync(request);
            //if (authResponse == null)
            //{
              //  return Unauthorized(new { message = "Invalid credentials" });
          //  }
            //return Ok(authResponse);
        //}

        [HttpGet("all")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
        }

        [HttpGet("email/{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
        }

        [HttpGet("username/{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByName(string username)
        {
            var user = await _userService.GetUserByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
        }

        [HttpPut("update-password")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePassword([FromBody] ResetPWDRequest request)
        {
            var result = await _userService.UpdatePasswordAsync(request);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { message = "Password updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { message = "User deleted successfully" });
        }

        [HttpPost("lock/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LockUser(string id)
        {
            var result = await _userService.LockUserAsync(id);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { message = "User locked successfully" });
        }

        [HttpPost("unlock/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var result = await _userService.UnlockUserAsync(id);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { message = "User unlocked successfully" });
        }
    }


