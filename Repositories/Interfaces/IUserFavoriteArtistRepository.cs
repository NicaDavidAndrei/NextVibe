using NextVibe.Models;

namespace NextVibe.Repositories.Interfaces
{
    public interface IUserFavoriteArtistRepository : IGenericRepository<UserFavoriteArtist>
    {
        Task<List<UserFavoriteArtist>> GetFavoritesByUserIdAsync(string userId);
        Task<List<UserFavoriteArtist>> GetAllWithDetailsAsync();
        Task<UserFavoriteArtist?> GetFavoriteAsync(string userId, int artistId);
        Task<bool> FavoriteExistsAsync(string userId, int artistId);
        Task<int> CountByUserIdAsync(string userId);
        Task<List<int>> GetFavoriteArtistIdsAsync(string userId);
    }
}