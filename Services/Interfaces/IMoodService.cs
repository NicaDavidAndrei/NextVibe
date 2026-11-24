using NextVibe.Models;

namespace NextVibe.Services.Interfaces
{
    public interface IMoodService
    {
        Task<List<Mood>> GetAllAsync();
        Task<Mood?> GetByIdAsync(int id);
        Task CreateAsync(Mood mood);
        Task UpdateAsync(Mood mood);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}