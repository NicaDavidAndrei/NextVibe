using Microsoft.EntityFrameworkCore;
using NextVibe.Data;
using NextVibe.Models;
using NextVibe.Repositories.Interfaces;

namespace NextVibe.Repositories
{
    public class ArtistAlbumRepository
        : GenericRepository<ArtistAlbum>,
          IArtistAlbumRepository
    {
        public ArtistAlbumRepository(AppDbContext context)
            : base(context)
        {
        }

        public override async Task<List<ArtistAlbum>> GetAllAsync()
        {
            return await _context.ArtistAlbums
                .Include(a => a.Artist)
                .Include(a => a.Tracks)
                .OrderByDescending(a => a.ReleaseYear)
                .ToListAsync();
        }

        public override async Task<ArtistAlbum?> GetByIdAsync(int id)
        {
            return await _context.ArtistAlbums
                .Include(a => a.Artist)
                .Include(a => a.Tracks)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<ArtistAlbum>> SearchAsync(string? searchString)
        {
            var query = _context.ArtistAlbums
                .Include(a => a.Artist)
                .Include(a => a.Tracks)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(a =>
                    a.Title.Contains(searchString) ||
                    a.Artist!.Name.Contains(searchString));
            }

            return await query
                .OrderByDescending(a => a.ReleaseYear)
                .ToListAsync();
        }
    }
}