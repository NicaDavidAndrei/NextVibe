using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextVibe.Data;
using NextVibe.Models;
using System.Diagnostics;

namespace NextVibe.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var trendingArtists = await _context.Artists
                .Include(a => a.Genre)
                .OrderBy(a => a.Name)
                .Take(3)
                .ToListAsync();

            return View(trendingArtists);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}