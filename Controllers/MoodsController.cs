using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextVibe.Models;
using NextVibe.Services.Interfaces;

namespace NextVibe.Controllers
{
    public class MoodsController : Controller
    {
        private readonly IMoodService _moodService;

        public MoodsController(IMoodService moodService)
        {
            _moodService = moodService;
        }

        // Public moods page
        public async Task<IActionResult> Index(string? searchString)
        {
            ViewBag.SearchString = searchString;

            var moods = await _moodService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                moods = moods
                    .Where(m =>
                        m.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        (!string.IsNullOrWhiteSpace(m.Description) &&
                         m.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            return View(moods);
        }

        // Public mood details page
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mood = await _moodService.GetByIdAsync(id.Value);

            if (mood == null)
            {
                return NotFound();
            }

            return View(mood);
        }

        // Admin CRUD table
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage()
        {
            var moods = await _moodService.GetAllAsync();
            return View(moods);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Mood mood)
        {
            if (ModelState.IsValid)
            {
                await _moodService.CreateAsync(mood);
                return RedirectToAction(nameof(Manage));
            }

            return View(mood);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mood = await _moodService.GetByIdAsync(id.Value);

            if (mood == null)
            {
                return NotFound();
            }

            return View(mood);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Mood mood)
        {
            if (id != mood.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _moodService.UpdateAsync(mood);
                return RedirectToAction(nameof(Manage));
            }

            return View(mood);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mood = await _moodService.GetByIdAsync(id.Value);

            if (mood == null)
            {
                return NotFound();
            }

            return View(mood);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _moodService.DeleteAsync(id);
            return RedirectToAction(nameof(Manage));
        }
    }
}