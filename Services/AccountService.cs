using Microsoft.AspNetCore.Identity;
using NextVibe.Models;
using NextVibe.Services.Interfaces;
using System.Security.Claims;

namespace NextVibe.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                var claims = new List<Claim>
                {
                    new Claim("FirstName", user.FirstName ?? "")
                };

                await _signInManager.SignInWithClaimsAsync(
                    user,
                    isPersistent: false,
                    claims);
            }

            return result;
        }

        public async Task<bool> LoginAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return false;
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return false;
            }

            var claims = new List<Claim>
            {
                new Claim("FirstName", user.FirstName ?? "")
            };

            await _signInManager.SignOutAsync();

            await _signInManager.SignInWithClaimsAsync(
                user,
                model.RememberMe,
                claims);

            return true;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            return await _userManager.GetUserAsync(user);
        }

        public async Task<bool> IsAdminAsync(ApplicationUser user)
        {
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<EditProfileViewModel?> GetEditProfileModelAsync(ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                return null;
            }

            return new EditProfileViewModel
            {
                FirstName = currentUser.FirstName ?? "",
                LastName = currentUser.LastName ?? "",
                Email = currentUser.Email ?? "",
                UserName = currentUser.UserName ?? ""
            };
        }

        public async Task<IdentityResult> UpdateProfileAsync(ClaimsPrincipal user, EditProfileViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Current user could not be found."
                    });
            }

            var emailOwner = await _userManager.FindByEmailAsync(model.Email);

            if (emailOwner != null && emailOwner.Id != currentUser.Id)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "This email is already used by another account."
                    });
            }

            var usernameOwner = await _userManager.FindByNameAsync(model.UserName);

            if (usernameOwner != null && usernameOwner.Id != currentUser.Id)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "This username is already taken."
                    });
            }

            currentUser.FirstName = model.FirstName;
            currentUser.LastName = model.LastName;
            currentUser.Email = model.Email;
            currentUser.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(currentUser);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(currentUser);
            }

            return result;
        }
    }
}