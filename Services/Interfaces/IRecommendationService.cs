using NextVibe.Models;

namespace NextVibe.Services.Interfaces
{
    public interface IRecommendationService
    {
        Task<List<Artist>> GetRecommendationsForUserAsync(string userId);
    }
}