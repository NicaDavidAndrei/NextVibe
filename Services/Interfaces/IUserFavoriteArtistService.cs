using NextVibe.Models;

namespace NextVibe.Services.Interfaces
{
    public interface IUserFavoriteArtistService
    {
        Task<List<UserFavoriteArtist>> GetFavoritesByUserIdAsync(string userId);
        Task<List<UserFavoriteArtist>> GetAllWithDetailsAsync();
        Task AddFavoriteAsync(string userId, int artistId);
        Task RemoveFavoriteAsync(string userId, int favoriteId);
        Task<int> CountByUserIdAsync(string userId);
        Task<List<int>> GetFavoriteArtistIdsAsync(string userId);
    }
}