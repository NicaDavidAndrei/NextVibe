using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextVibe.Data;

namespace NextVibe.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var usersCount = await _context.Users.CountAsync();
            var artistsCount = await _context.Artists.CountAsync();
            var genresCount = await _context.Genres.CountAsync();
            var moodsCount = await _context.Moods.CountAsync();
            var favoritesCount = await _context.UserFavoriteArtists.CountAsync();
            var artistMoodsCount = await _context.ArtistMoods.CountAsync();
            var albumsCount = await _context.ArtistAlbums.CountAsync();
            var tracksCount = await _context.AlbumTracks.CountAsync();

            ViewBag.UsersCount = usersCount;
            ViewBag.ArtistsCount = artistsCount;
            ViewBag.GenresCount = genresCount;
            ViewBag.MoodsCount = moodsCount;
            ViewBag.FavoritesCount = favoritesCount;
            ViewBag.ArtistMoodsCount = artistMoodsCount;
            ViewBag.AlbumsCount = albumsCount;
            ViewBag.TracksCount = tracksCount;

            return View();
        }
    }
}