using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextVibe.Data;
using NextVibe.Models;
using NextVibe.Services.Interfaces;

namespace NextVibe.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly AppDbContext _context;

        public AccountController(
            IAccountService accountService,
            AppDbContext context)
        {
            _accountService = accountService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _accountService.RegisterAsync(model);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loggedIn = await _accountService.LoginAsync(model);

            if (loggedIn)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var currentUser = await _accountService.GetCurrentUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var favoriteCount = await _context.UserFavoriteArtists
                .CountAsync(f => f.UserId == currentUser.Id);

            var favoriteArtistIds = await _context.UserFavoriteArtists
                .Where(f => f.UserId == currentUser.Id)
                .Select(f => f.ArtistId)
                .ToListAsync();

            var recommendationCount = 0;

            if (favoriteArtistIds.Any())
            {
                var favoriteGenreIds = await _context.Artists
                    .Where(a => favoriteArtistIds.Contains(a.Id))
                    .Select(a => a.GenreId)
                    .Distinct()
                    .ToListAsync();

                recommendationCount = await _context.Artists
                    .CountAsync(a =>
                        favoriteGenreIds.Contains(a.GenreId)
                        && !favoriteArtistIds.Contains(a.Id));
            }

            ViewBag.FavoriteCount = favoriteCount;
            ViewBag.RecommendationCount = recommendationCount;
            ViewBag.IsAdmin = await _accountService.IsAdminAsync(currentUser);

            return View(currentUser);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var model = await _accountService.GetEditProfileModelAsync(User);

            if (model == null)
            {
                return RedirectToAction(nameof(Login));
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _accountService.UpdateProfileAsync(User, model);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    if (error.Description.Contains("email", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(nameof(model.Email), error.Description);
                    }
                    else if (error.Description.Contains("username", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(nameof(model.UserName), error.Description);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                return View(model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}