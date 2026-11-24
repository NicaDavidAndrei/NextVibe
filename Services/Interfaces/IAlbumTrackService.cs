using NextVibe.Models;

namespace NextVibe.Services.Interfaces
{
    public interface IAlbumTrackService
    {
        Task<List<AlbumTrack>> GetAllAsync();
        Task<AlbumTrack?> GetByIdAsync(int id);
        Task CreateAsync(AlbumTrack track);
        Task UpdateAsync(AlbumTrack track);
        Task DeleteAsync(int id);
    }
}