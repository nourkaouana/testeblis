using testbills.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using testbills.Data;


namespace testbills.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser>? _userManager;
        private readonly ApplicationDbContext? _context;
        private readonly TokenService? _tokenService;
        private readonly IHubContext<NotificationHub>? _hubContext;

        public UserService(
             UserManager<ApplicationUser> userManager,
             ApplicationDbContext context,
             TokenService tokenService,
             IHubContext<NotificationHub> hubContext)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
            _hubContext = hubContext;
        }
        public async Task<IdentityResult> RegisterUserAsync(RegistrationRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Username,
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                Email = request.Email,
                Role = request.Role,
                CreatedDate = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                // Send notification
                await _hubContext.Clients.All.SendAsync("UserRegistered", user);
            }

            return result;
        }

        public async Task<AuthResponse> AuthenticateUserAsync(AuthRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return null;
            }

            var token = _tokenService.CreateToken(user);

            // Send notification
            await _hubContext.Clients.All.SendAsync("UserAuthenticated", user);

            return new AuthResponse { Username = user.UserName, Email = user.Email, Token = token };
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> GetUserByNameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<IdentityResult> UpdatePasswordAsync(ResetPWDRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded) return removePasswordResult;

            var addPasswordResult = await _userManager.AddPasswordAsync(user, request.Newpassword);

            if (addPasswordResult.Succeeded)
            {
                // Send notification
                await _hubContext.Clients.All.SendAsync("UserPasswordUpdated", user);
            }

            return addPasswordResult;
        }

        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                // Send notification
                await _hubContext.Clients.All.SendAsync("UserDeleted", id);
            }

            return result;
        }

        public async Task<IdentityResult> LockUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            user.LockoutEnd = DateTimeOffset.MaxValue;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Send notification
                await _hubContext.Clients.All.SendAsync("UserLocked", id);
            }

            return result;
        }

        public async Task<IdentityResult> UnlockUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            user.LockoutEnd = null;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Send notification
                await _hubContext.Clients.All.SendAsync("UserUnlocked", id);
            }

            return result;
        }
    }
}
