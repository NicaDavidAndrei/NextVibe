using NextVibe.Models;

namespace NextVibe.Repositories.Interfaces
{
    public interface IArtistAlbumRepository : IGenericRepository<ArtistAlbum>
    {
        Task<List<ArtistAlbum>> SearchAsync(string? searchString);
    }
}