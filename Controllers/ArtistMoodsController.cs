using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NextVibe.Models;
using NextVibe.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace NextVibe.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArtistMoodsController : Controller
    {
        private readonly IArtistMoodService _artistMoodService;
        private readonly IArtistService _artistService;
        private readonly IMoodService _moodService;

        public ArtistMoodsController(
            IArtistMoodService artistMoodService,
            IArtistService artistService,
            IMoodService moodService)
        {
            _artistMoodService = artistMoodService;
            _artistService = artistService;
            _moodService = moodService;
        }

        public async Task<IActionResult> Index()
        {
            var artists = await _artistService.GetAllAsync();
            return View(artists);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artistMood = await _artistMoodService.GetByIdAsync(id.Value);
            if (artistMood == null)
            {
                return NotFound();
            }

            return View(artistMood);
        }

        public async Task<IActionResult> Create()
        {
            var artists = await _artistService.GetAllAsync();
            var moods = await _moodService.GetAllAsync();

            ViewData["ArtistId"] = new SelectList(artists, "Id", "Name");
            ViewData["MoodId"] = new SelectList(moods, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ArtistId,MoodId")] ArtistMood artistMood)
        {
            if (ModelState.IsValid)
            {
                var created = await _artistMoodService.CreateAsync(artistMood);

                if (created)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "This artist-mood relation already exists.");
            }

            var artists = await _artistService.GetAllAsync();
            var moods = await _moodService.GetAllAsync();

            ViewData["ArtistId"] = new SelectList(artists, "Id", "Name", artistMood.ArtistId);
            ViewData["MoodId"] = new SelectList(moods, "Id", "Name", artistMood.MoodId);

            return View(artistMood);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artistMood = await _artistMoodService.GetByIdAsync(id.Value);
            if (artistMood == null)
            {
                return NotFound();
            }

            var artists = await _artistService.GetAllAsync();
            var moods = await _moodService.GetAllAsync();

            ViewData["ArtistId"] = new SelectList(artists, "Id", "Name", artistMood.ArtistId);
            ViewData["MoodId"] = new SelectList(moods, "Id", "Name", artistMood.MoodId);

            return View(artistMood);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArtistId,MoodId")] ArtistMood artistMood)
        {
            if (id != artistMood.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var updated = await _artistMoodService.UpdateAsync(artistMood);

                if (updated)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "This artist-mood relation already exists.");
            }

            var artists = await _artistService.GetAllAsync();
            var moods = await _moodService.GetAllAsync();

            ViewData["ArtistId"] = new SelectList(artists, "Id", "Name", artistMood.ArtistId);
            ViewData["MoodId"] = new SelectList(moods, "Id", "Name", artistMood.MoodId);

            return View(artistMood);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artistMood = await _artistMoodService.GetByIdAsync(id.Value);

            if (artistMood == null)
            {
                return NotFound();
            }

            return View(artistMood);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _artistMoodService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}