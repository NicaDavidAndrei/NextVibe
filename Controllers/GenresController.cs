using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextVibe.Models;
using NextVibe.Services.Interfaces;

namespace NextVibe.Controllers
{
    public class GenresController : Controller
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        // Public genres page
        public async Task<IActionResult> Index(string? searchString)
        {
            ViewBag.SearchString = searchString;

            var genres = await _genreService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                genres = genres
                    .Where(g =>
                        g.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        (!string.IsNullOrWhiteSpace(g.Description) &&
                         g.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            return View(genres);
        }

        // Public genre details page
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _genreService.GetByIdAsync(id.Value);

            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // Admin CRUD table
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage()
        {
            var genres = await _genreService.GetAllAsync();
            return View(genres);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                await _genreService.CreateAsync(genre);
                return RedirectToAction(nameof(Manage));
            }

            return View(genre);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _genreService.GetByIdAsync(id.Value);

            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Genre genre)
        {
            if (id != genre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _genreService.UpdateAsync(genre);
                return RedirectToAction(nameof(Manage));
            }

            return View(genre);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _genreService.GetByIdAsync(id.Value);

            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _genreService.DeleteAsync(id);
            return RedirectToAction(nameof(Manage));
        }
    }
}