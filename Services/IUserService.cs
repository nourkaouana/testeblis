using testbills.Models;
using Microsoft.AspNetCore.Identity;

namespace testbills.Services
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegistrationRequest request);
        Task<AuthResponse> AuthenticateUserAsync(AuthRequest request);
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<ApplicationUser> GetUserByNameAsync(string username);
        Task<IdentityResult> UpdatePasswordAsync(ResetPWDRequest request);
        Task<IdentityResult> DeleteUserAsync(string id);
        Task<IdentityResult> LockUserAsync(string id);
        Task<IdentityResult> UnlockUserAsync(string id);
    }
}
