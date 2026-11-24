using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextVibe.Models;
using NextVibe.Services.Interfaces;

namespace NextVibe.Controllers
{
    [Authorize]
    public class RecommendationsController : Controller
    {
        private readonly IRecommendationService _recommendationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RecommendationsController(
            IRecommendationService recommendationService,
            UserManager<ApplicationUser> userManager)
        {
            _recommendationService = recommendationService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.UserType = User.IsInRole("Admin") ? "Admin" : "User";

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var recommendations = await _recommendationService
                .GetRecommendationsForUserAsync(currentUser.Id);

            return View(recommendations);
        }
    }
}