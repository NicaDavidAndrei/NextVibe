using NextVibe.Models;

namespace NextVibe.Repositories.Interfaces
{
    public interface IArtistRepository : IGenericRepository<Artist>
    {
        Task<List<Artist>> SearchAsync(string? searchString, int? genreId, int? moodId);
        Task<List<Artist>> GetRecommendationsByFavoriteGenresAsync(List<int> favoriteGenreIds, List<int> favoriteArtistIds);
    }
}