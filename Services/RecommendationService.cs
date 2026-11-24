using NextVibe.Models;
using NextVibe.Repositories.Interfaces;
using NextVibe.Services.Interfaces;

namespace NextVibe.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IUserFavoriteArtistRepository _favoriteRepository;
        private readonly IArtistRepository _artistRepository;

        public RecommendationService(
            IUserFavoriteArtistRepository favoriteRepository,
            IArtistRepository artistRepository)
        {
            _favoriteRepository = favoriteRepository;
            _artistRepository = artistRepository;
        }

        public async Task<List<Artist>> GetRecommendationsForUserAsync(string userId)
        {
            var favorites = await _favoriteRepository.GetFavoritesByUserIdAsync(userId);

            if (favorites == null || !favorites.Any())
            {
                return new List<Artist>();
            }

            var favoriteArtistIds = favorites
                .Select(f => f.ArtistId)
                .Distinct()
                .ToList();

            var favoriteGenreIds = favorites
                .Where(f => f.Artist != null)
                .Where(f => f.Artist!.GenreId != 0)
                .Select(f => f.Artist!.GenreId)
                .Distinct()
                .ToList();

            if (!favoriteGenreIds.Any())
            {
                return new List<Artist>();
            }

            var recommendations = await _artistRepository
                .GetRecommendationsByFavoriteGenresAsync(favoriteGenreIds, favoriteArtistIds);

            return recommendations;
        }
    }
}