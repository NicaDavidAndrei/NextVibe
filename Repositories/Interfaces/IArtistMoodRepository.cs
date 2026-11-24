using NextVibe.Models;

namespace NextVibe.Repositories.Interfaces
{
    public interface IArtistMoodRepository : IGenericRepository<ArtistMood>
    {
        Task<bool> ExistsAsync(int artistId, int moodId);
    }
}