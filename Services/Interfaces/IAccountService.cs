using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using NextVibe.Models;

namespace NextVibe.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<bool> LoginAsync(LoginViewModel model);
        Task LogoutAsync();

        Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal user);
        Task<bool> IsAdminAsync(ApplicationUser user);

        Task<EditProfileViewModel?> GetEditProfileModelAsync(ClaimsPrincipal user);
        Task<IdentityResult> UpdateProfileAsync(ClaimsPrincipal user, EditProfileViewModel model);
    }
}