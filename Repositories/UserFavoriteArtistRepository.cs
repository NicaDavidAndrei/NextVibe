using Microsoft.EntityFrameworkCore;
using NextVibe.Data;
using NextVibe.Models;
using NextVibe.Repositories.Interfaces;

namespace NextVibe.Repositories
{
    public class UserFavoriteArtistRepository : GenericRepository<UserFavoriteArtist>, IUserFavoriteArtistRepository
    {
        public UserFavoriteArtistRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<UserFavoriteArtist>> GetFavoritesByUserIdAsync(string userId)
        {
            return await _context.UserFavoriteArtists
                .Include(f => f.Artist)
                    .ThenInclude(a => a!.Genre)
                .Include(f => f.User)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.AddedAt)
                .ToListAsync();
        }

        public async Task<List<UserFavoriteArtist>> GetAllWithDetailsAsync()
        {
            return await _context.UserFavoriteArtists
                .Include(f => f.Artist)
                .Include(f => f.User)
                .OrderByDescending(f => f.AddedAt)
                .ToListAsync();
        }

        public async Task<UserFavoriteArtist?> GetFavoriteAsync(string userId, int artistId)
        {
            return await _context.UserFavoriteArtists
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ArtistId == artistId);
        }

        public async Task<bool> FavoriteExistsAsync(string userId, int artistId)
        {
            return await _context.UserFavoriteArtists
                .AnyAsync(f => f.UserId == userId && f.ArtistId == artistId);
        }

        public async Task<int> CountByUserIdAsync(string userId)
        {
            return await _context.UserFavoriteArtists
                .CountAsync(f => f.UserId == userId);
        }

        public async Task<List<int>> GetFavoriteArtistIdsAsync(string userId)
        {
            return await _context.UserFavoriteArtists
                .Where(f => f.UserId == userId)
                .Select(f => f.ArtistId)
                .ToListAsync();
        }
    }
}