using Microsoft.EntityFrameworkCore;
using NextVibe.Data;
using NextVibe.Models;
using NextVibe.Repositories.Interfaces;

namespace NextVibe.Repositories
{
    public class ArtistMoodRepository
        : GenericRepository<ArtistMood>,
          IArtistMoodRepository
    {
        public ArtistMoodRepository(AppDbContext context)
            : base(context)
        {
        }

        public override async Task<List<ArtistMood>> GetAllAsync()
        {
            return await _context.ArtistMoods
                .Include(am => am.Artist)
                .Include(am => am.Mood)
                .OrderBy(am => am.Artist!.Name)
                .ToListAsync();
        }

        public override async Task<ArtistMood?> GetByIdAsync(int id)
        {
            return await _context.ArtistMoods
                .Include(am => am.Artist)
                .Include(am => am.Mood)
                .FirstOrDefaultAsync(am => am.Id == id);
        }

        public async Task<bool> ExistsAsync(int artistId, int moodId)
        {
            return await _context.ArtistMoods
                .AnyAsync(am =>
                    am.ArtistId == artistId &&
                    am.MoodId == moodId);
        }
    }
}