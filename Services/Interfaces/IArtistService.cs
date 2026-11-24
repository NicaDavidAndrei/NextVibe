using NextVibe.Models;

namespace NextVibe.Services.Interfaces
{
    public interface IArtistService
    {
        Task<List<Artist>> GetAllAsync();
        Task<Artist?> GetByIdAsync(int id);
        Task<List<Artist>> SearchAsync(string? searchString, int? genreId, int? moodId);
        Task CreateAsync(Artist artist);
        Task UpdateAsync(Artist artist);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}