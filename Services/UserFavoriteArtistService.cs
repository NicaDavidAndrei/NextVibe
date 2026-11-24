using NextVibe.Models;
using NextVibe.Repositories.Interfaces;
using NextVibe.Services.Interfaces;

namespace NextVibe.Services
{
    public class UserFavoriteArtistService : IUserFavoriteArtistService
    {
        private readonly IUserFavoriteArtistRepository _favoriteRepository;
        private readonly IArtistRepository _artistRepository;

        public UserFavoriteArtistService(
            IUserFavoriteArtistRepository favoriteRepository,
            IArtistRepository artistRepository)
        {
            _favoriteRepository = favoriteRepository;
            _artistRepository = artistRepository;
        }

        public async Task<List<UserFavoriteArtist>> GetFavoritesByUserIdAsync(string userId)
        {
            return await _favoriteRepository.GetFavoritesByUserIdAsync(userId);
        }

        public async Task<List<UserFavoriteArtist>> GetAllWithDetailsAsync()
        {
            return await _favoriteRepository.GetAllWithDetailsAsync();
        }

        public async Task AddFavoriteAsync(string userId, int artistId)
        {
            var artist = await _artistRepository.GetByIdAsync(artistId);

            if (artist == null)
            {
                return;
            }

            var alreadyExists = await _favoriteRepository.FavoriteExistsAsync(userId, artistId);

            if (!alreadyExists)
            {
                var favorite = new UserFavoriteArtist
                {
                    UserId = userId,
                    ArtistId = artistId,
                    AddedAt = DateTime.Now
                };

                await _favoriteRepository.AddAsync(favorite);
            }
        }

        public async Task RemoveFavoriteAsync(string userId, int favoriteId)
        {
            var favorite = await _favoriteRepository.GetByIdAsync(favoriteId);

            if (favorite != null && favorite.UserId == userId)
            {
                await _favoriteRepository.DeleteAsync(favorite.Id);
            }
        }

        public async Task<int> CountByUserIdAsync(string userId)
        {
            return await _favoriteRepository.CountByUserIdAsync(userId);
        }

        public async Task<List<int>> GetFavoriteArtistIdsAsync(string userId)
        {
            return await _favoriteRepository.GetFavoriteArtistIdsAsync(userId);
        }
    }
}