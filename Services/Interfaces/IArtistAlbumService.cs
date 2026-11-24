using NextVibe.Models;

namespace NextVibe.Services.Interfaces
{
    public interface IArtistAlbumService
    {
        Task<List<ArtistAlbum>> GetAllAsync();
        Task<ArtistAlbum?> GetByIdAsync(int id);
        Task<List<ArtistAlbum>> SearchAsync(string? searchString);
        Task CreateAsync(ArtistAlbum album);
        Task UpdateAsync(ArtistAlbum album);
        Task DeleteAsync(int id);
    }
}