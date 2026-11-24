using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextVibe.Models;
using NextVibe.Services.Interfaces;

namespace NextVibe.Controllers
{
    [Authorize]
    public class UserFavoriteArtistsController : Controller
    {
        private readonly IUserFavoriteArtistService _favoriteService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserFavoriteArtistsController(
            IUserFavoriteArtistService favoriteService,
            UserManager<ApplicationUser> userManager)
        {
            _favoriteService = favoriteService;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyVibes()
        {
            ViewBag.UserType = User.IsInRole("Admin") ? "Admin" : "User";

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var favorites = await _favoriteService.GetFavoritesByUserIdAsync(currentUser.Id);

            return View(favorites);
        }

        public async Task<IActionResult> Add(int artistId)
        {
            ViewBag.UserType = User.IsInRole("Admin") ? "Admin" : "User";

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            await _favoriteService.AddFavoriteAsync(currentUser.Id, artistId);

            return RedirectToAction(nameof(MyVibes));
        }

        public async Task<IActionResult> Remove(int id)
        {
            ViewBag.UserType = User.IsInRole("Admin") ? "Admin" : "User";

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            await _favoriteService.RemoveFavoriteAsync(currentUser.Id, id);

            return RedirectToAction(nameof(MyVibes));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            ViewBag.UserType = "Admin";

            var favorites = await _favoriteService.GetAllWithDetailsAsync();

            return View(favorites);
        }
    }
}