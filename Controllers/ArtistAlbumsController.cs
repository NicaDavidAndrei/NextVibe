using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NextVibe.Models;
using NextVibe.Services.Interfaces;

namespace NextVibe.Controllers
{
    public class ArtistAlbumsController : Controller
    {
        private readonly IArtistAlbumService _albumService;
        private readonly IArtistService _artistService;

        public ArtistAlbumsController(
            IArtistAlbumService albumService,
            IArtistService artistService)
        {
            _albumService = albumService;
            _artistService = artistService;
        }

        // Public page
        public async Task<IActionResult> Index(string? searchString)
        {
            ViewBag.SearchString = searchString;

            var albums = await _albumService.SearchAsync(searchString);
            return View(albums);
        }

        // Public details page
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _albumService.GetByIdAsync(id.Value);

            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // Admin CRUD table
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage(string? searchString)
        {
            ViewBag.SearchString = searchString;

            var albums = await _albumService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                albums = albums
                    .Where(a =>
                        a.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        (a.Artist != null && a.Artist.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(a.Description) && a.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            return View(albums);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var artists = await _artistService.GetAllAsync();
            ViewData["ArtistId"] = new SelectList(artists, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            [Bind("Id,Title,ReleaseYear,Description,CoverImageUrl,ArtistId")] ArtistAlbum album)
        {
            if (ModelState.IsValid)
            {
                await _albumService.CreateAsync(album);
                return RedirectToAction(nameof(Manage));
            }

            var artists = await _artistService.GetAllAsync();
            ViewData["ArtistId"] = new SelectList(artists, "Id", "Name", album.ArtistId);

            return View(album);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _albumService.GetByIdAsync(id.Value);

            if (album == null)
            {
                return NotFound();
            }

            var artists = await _artistService.GetAllAsync();
            ViewData["ArtistId"] = new SelectList(artists, "Id", "Name", album.ArtistId);

            return View(album);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Title,ReleaseYear,Description,CoverImageUrl,ArtistId")] ArtistAlbum album)
        {
            if (id != album.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _albumService.UpdateAsync(album);
                return RedirectToAction(nameof(Manage));
            }

            var artists = await _artistService.GetAllAsync();
            ViewData["ArtistId"] = new SelectList(artists, "Id", "Name", album.ArtistId);

            return View(album);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _albumService.GetByIdAsync(id.Value);

            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _albumService.DeleteAsync(id);
            return RedirectToAction(nameof(Manage));
        }
    }
}