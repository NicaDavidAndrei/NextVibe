using Microsoft.EntityFrameworkCore;
using NextVibe.Data;
using NextVibe.Models;
using NextVibe.Repositories.Interfaces;

namespace NextVibe.Repositories
{
    public class AlbumTrackRepository : GenericRepository<AlbumTrack>, IAlbumTrackRepository
    {
        public AlbumTrackRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<List<AlbumTrack>> GetAllAsync()
        {
            return await _context.AlbumTracks
                .Include(t => t.ArtistAlbum)
                    .ThenInclude(a => a!.Artist)
                .OrderBy(t => t.ArtistAlbum!.Title)
                .ThenBy(t => t.TrackNumber)
                .ToListAsync();
        }

        public override async Task<AlbumTrack?> GetByIdAsync(int id)
        {
            return await _context.AlbumTracks
                .Include(t => t.ArtistAlbum)
                    .ThenInclude(a => a!.Artist)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}