using NextVibe.Models;

namespace NextVibe.Services.Interfaces
{
    public interface IArtistMoodService
    {
        Task<List<ArtistMood>> GetAllAsync();
        Task<ArtistMood?> GetByIdAsync(int id);
        Task<bool> CreateAsync(ArtistMood artistMood);
        Task<bool> UpdateAsync(ArtistMood artistMood);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}