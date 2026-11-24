using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NextVibe.Data;
using NextVibe.Models;
using NextVibe.Services.Interfaces;

namespace NextVibe.Controllers
{
    public class ExploreController : Controller
    {
        private readonly IArtistService _artistService;
        private readonly IGenreService _genreService;
        private readonly IMoodService _moodService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public ExploreController(
            IArtistService artistService,
            IGenreService genreService,
            IMoodService moodService,
            UserManager<ApplicationUser> userManager,
            AppDbContext context)
        {
            _artistService = artistService;
            _genreService = genreService;
            _moodService = moodService;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchString, int? genreId, int? moodId)
        {
            ViewBag.SearchString = searchString;
            ViewBag.GenreId = genreId;
            ViewBag.MoodId = moodId;

            var genres = await _genreService.GetAllAsync();
            ViewBag.Genres = new SelectList(genres, "Id", "Name", genreId);

            var moods = await _moodService.GetAllAsync();
            ViewBag.Moods = new SelectList(moods, "Id", "Name", moodId);

            var artists = await _artistService.SearchAsync(searchString, genreId, moodId);

            ViewBag.FavoriteArtistIds = await GetFavoriteArtistIdsAsync();

            return View(artists);
        }

        [HttpGet]
        public async Task<IActionResult> FilterArtists(string? searchString, int? genreId, int? moodId)
        {
            var artists = await _artistService.SearchAsync(searchString, genreId, moodId);

            ViewBag.FavoriteArtistIds = await GetFavoriteArtistIdsAsync();

            return PartialView("_ArtistCards", artists);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _artistService.GetByIdAsync(id.Value);

            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        private async Task<List<int>> GetFavoriteArtistIdsAsync()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return new List<int>();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return new List<int>();
            }

            return await _context.UserFavoriteArtists
                .Where(f => f.UserId == currentUser.Id)
                .Select(f => f.ArtistId)
                .ToListAsync();
        }
    }
}