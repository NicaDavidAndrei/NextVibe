using Microsoft.EntityFrameworkCore;
using NextVibe.Data;
using NextVibe.Models;
using NextVibe.Repositories.Interfaces;

namespace NextVibe.Repositories
{
    public class ArtistRepository : GenericRepository<Artist>, IArtistRepository
    {
        public ArtistRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<List<Artist>> GetAllAsync()
        {
            return await _context.Artists
                .Include(a => a.Genre)
                .Include(a => a.ArtistMoods)
                    .ThenInclude(am => am.Mood)
                .Include(a => a.Albums)
                    .ThenInclude(al => al.Tracks)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public override async Task<Artist?> GetByIdAsync(int id)
        {
            return await _context.Artists
                .Include(a => a.Genre)
                .Include(a => a.ArtistMoods)
                    .ThenInclude(am => am.Mood)
                .Include(a => a.Albums)
                    .ThenInclude(al => al.Tracks)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Artist>> SearchAsync(string? searchString, int? genreId, int? moodId)
        {
            var query = _context.Artists
                .Include(a => a.Genre)
                .Include(a => a.ArtistMoods)
                    .ThenInclude(am => am.Mood)
                .Include(a => a.Albums)
                    .ThenInclude(al => al.Tracks)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(a =>
                    a.Name.Contains(searchString) ||
                    (a.Country != null && a.Country.Contains(searchString)) ||
                    (a.Bio != null && a.Bio.Contains(searchString)));
            }

            if (genreId.HasValue)
            {
                query = query.Where(a => a.GenreId == genreId.Value);
            }

            if (moodId.HasValue)
            {
                query = query.Where(a =>
                    a.ArtistMoods.Any(am => am.MoodId == moodId.Value));
            }

            return await query
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<List<Artist>> GetRecommendationsByFavoriteGenresAsync(
            List<int> favoriteGenreIds,
            List<int> favoriteArtistIds)
        {
            return await _context.Artists
                .Include(a => a.Genre)
                .Include(a => a.ArtistMoods)
                    .ThenInclude(am => am.Mood)
                .Include(a => a.Albums)
                    .ThenInclude(al => al.Tracks)
                .Where(a =>
                    favoriteGenreIds.Contains(a.GenreId) &&
                    !favoriteArtistIds.Contains(a.Id))
                .OrderBy(a => a.Name)
                .ToListAsync();
        }
    }
}