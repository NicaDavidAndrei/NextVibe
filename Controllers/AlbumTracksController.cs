using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NextVibe.Data;
using NextVibe.Models;
using NextVibe.Services.Interfaces;

namespace NextVibe.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AlbumTracksController : Controller
    {
        private readonly IAlbumTrackService _albumTrackService;
        private readonly AppDbContext _context;

        public AlbumTracksController(
            IAlbumTrackService albumTrackService,
            AppDbContext context)
        {
            _albumTrackService = albumTrackService;
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchString)
        {
            ViewBag.SearchString = searchString;

            var tracks = await _albumTrackService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                tracks = tracks
                    .Where(t =>
                        t.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        (t.ArtistAlbum != null && t.ArtistAlbum.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (t.ArtistAlbum?.Artist != null && t.ArtistAlbum.Artist.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            return View(tracks);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _albumTrackService.GetByIdAsync(id.Value);

            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        public IActionResult Create()
        {
            LoadAlbumsDropDown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlbumTrack track)
        {
            if (ModelState.IsValid)
            {
                await _albumTrackService.CreateAsync(track);
                return RedirectToAction(nameof(Index));
            }

            LoadAlbumsDropDown(track.ArtistAlbumId);
            return View(track);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _albumTrackService.GetByIdAsync(id.Value);

            if (track == null)
            {
                return NotFound();
            }

            LoadAlbumsDropDown(track.ArtistAlbumId);
            return View(track);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AlbumTrack track)
        {
            if (id != track.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _albumTrackService.UpdateAsync(track);
                return RedirectToAction(nameof(Index));
            }

            LoadAlbumsDropDown(track.ArtistAlbumId);
            return View(track);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _albumTrackService.GetByIdAsync(id.Value);

            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _albumTrackService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private void LoadAlbumsDropDown(int? selectedAlbumId = null)
        {
            var albums = _context.ArtistAlbums
                .Include(a => a.Artist)
                .OrderBy(a => a.Artist!.Name)
                .ThenBy(a => a.ReleaseYear)
                .Select(a => new
                {
                    a.Id,
                    DisplayName = a.Artist!.Name + " - " + a.Title
                })
                .ToList();

            ViewData["ArtistAlbumId"] = new SelectList(albums, "Id", "DisplayName", selectedAlbumId);
        }
    }
}