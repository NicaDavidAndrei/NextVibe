using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NextVibe.Models;
using NextVibe.Services;
using NextVibe.Services.Interfaces;

namespace NextVibe.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArtistsController : Controller
    {
        private readonly IArtistService _artistService;
        private readonly IGenreService _genreService;

        public ArtistsController(IArtistService artistService, IGenreService genreService)
        {
            _artistService = artistService;
            _genreService = genreService;
        }

        public async Task<IActionResult> Index(string? searchString, int? genreId)
        {
            ViewBag.SearchString = searchString;
            ViewBag.SelectedGenreId = genreId;

            ViewBag.Genres = new SelectList(
                await _genreService.GetAllAsync(),
                "Id",
                "Name",
                genreId
            );

            var artists = await _artistService.SearchAsync(searchString, genreId, null);

            return View(artists);
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

        public async Task<IActionResult> Create()
        {
            var genres = await _genreService.GetAllAsync();
            ViewData["GenreId"] = new SelectList(genres, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Bio,ImageUrl,Country,BirthYear,GenreId")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                await _artistService.CreateAsync(artist);
                return RedirectToAction(nameof(Index));
            }

            var genres = await _genreService.GetAllAsync();
            ViewData["GenreId"] = new SelectList(genres, "Id", "Name", artist.GenreId);
            return View(artist);
        }

        public async Task<IActionResult> Edit(int? id)
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

            var genres = await _genreService.GetAllAsync();
            ViewData["GenreId"] = new SelectList(genres, "Id", "Name", artist.GenreId);
            return View(artist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Bio,ImageUrl,Country,BirthYear,GenreId")] Artist artist)
        {
            if (id != artist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _artistService.UpdateAsync(artist);
                return RedirectToAction(nameof(Index));
            }

            var genres = await _genreService.GetAllAsync();
            ViewData["GenreId"] = new SelectList(genres, "Id", "Name", artist.GenreId);
            return View(artist);
        }

        public async Task<IActionResult> Delete(int? id)
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _artistService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}