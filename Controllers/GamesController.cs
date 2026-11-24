using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextVibe.Models;
using NextVibe.Models.ViewModels.Games;
using NextVibe.Services.Interfaces;

namespace NextVibe.Controllers
{
    public class GamesController : Controller
    {
        private readonly IGamesService _gamesService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GamesController(
            IGamesService gamesService,
            UserManager<ApplicationUser> userManager)
        {
            _gamesService = gamesService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _gamesService.GetDashboardDataAsync(userId);
            return View(model);
        }

        public IActionResult GuessSong(int? artistId, string inputMode = "choice")
        {
            ViewBag.ArtistId = artistId;
            ViewBag.InputMode = string.Equals(inputMode, "type", StringComparison.OrdinalIgnoreCase) ? "type" : "choice";
            return View();
        }

        public IActionResult GuessAlbum(string mode = "random", int? artistId = null)
        {
            ViewBag.Mode = string.IsNullOrEmpty(mode) ? "random" : mode.ToLowerInvariant();
            ViewBag.ArtistId = artistId;
            return View();
        }

        public IActionResult FinishLyric(string difficulty = "Easy", int? artistId = null)
        {
            ViewBag.Difficulty = string.IsNullOrEmpty(difficulty) ? "Easy" : difficulty;
            ViewBag.ArtistId = artistId;
            return View();
        }

        public async Task<IActionResult> DailyChallenge()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool alreadyPlayed = false;

            if (!string.IsNullOrEmpty(userId))
            {
                alreadyPlayed = await _gamesService.HasPlayedDailyTodayAsync(userId);
            }

            ViewBag.AlreadyPlayed = alreadyPlayed;
            return View();
        }

        public async Task<IActionResult> Stats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = await _gamesService.GetUserGameStatsAsync(userId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetSongRounds(int? artistId, int count = 5)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rounds = await _gamesService.GenerateGuessSongRoundsAsync(artistId, userId, count);
            return Json(rounds);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSongTitles(int? artistId = null)
        {
            var titles = await _gamesService.GetAllSongTitlesAsync(artistId);
            return Json(titles);
        }

        [HttpGet]
        public async Task<IActionResult> SearchSongs(string q, int? artistId = null)
        {
            var results = await _gamesService.SearchSongsAsync(q, artistId);
            return Json(results);
        }

        [HttpGet]
        public async Task<IActionResult> GetAlbumRounds(string mode = "random", int? artistId = null, int count = 5)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rounds = await _gamesService.GenerateGuessAlbumRoundsAsync(mode, artistId, userId, count);
            return Json(rounds);
        }

        [HttpGet]
        public async Task<IActionResult> GetLyricRounds(string difficulty = "Easy", int? artistId = null, int count = 5)
        {
            var rounds = await _gamesService.GenerateFinishLyricRoundsAsync(difficulty, artistId, count);
            return Json(rounds);
        }

        [HttpGet]
        public async Task<IActionResult> GetDailyRounds()
        {
            var rounds = await _gamesService.GenerateDailyChallengeRoundsAsync();
            return Json(rounds);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitScore([FromBody] SubmitGameScoreDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid score payload");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If it's a daily challenge and user already played, prevent duplicate score
            if (dto.GameType == "DailyChallenge" && !string.IsNullOrEmpty(userId))
            {
                if (await _gamesService.HasPlayedDailyTodayAsync(userId))
                {
                    return Json(new { success = false, message = "You already played today's Daily Challenge!" });
                }
            }

            var saved = await _gamesService.SaveGameScoreAsync(userId, dto);
            return Json(new { success = true, scoreId = saved.Id, score = saved.Score });
        }

        public async Task<IActionResult> Leaderboard(string? gameType)
        {
            var scores = await _gamesService.GetLeaderboardAsync(gameType, 20);
            ViewBag.CurrentGameType = gameType;
            return View(scores);
        }
    }
}
