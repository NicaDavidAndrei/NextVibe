using NextVibe.Models;
using NextVibe.Repositories.Interfaces;
using NextVibe.Services.Interfaces;

namespace NextVibe.Services
{
    public class ArtistAlbumService : IArtistAlbumService
    {
        private readonly IArtistAlbumRepository _albumRepository;

        public ArtistAlbumService(IArtistAlbumRepository albumRepository)
        {
            _albumRepository = albumRepository;
        }

        public async Task<List<ArtistAlbum>> GetAllAsync()
        {
            return await _albumRepository.GetAllAsync();
        }

        public async Task<ArtistAlbum?> GetByIdAsync(int id)
        {
            return await _albumRepository.GetByIdAsync(id);
        }

        public async Task<List<ArtistAlbum>> SearchAsync(string? searchString)
        {
            return await _albumRepository.SearchAsync(searchString);
        }

        public async Task CreateAsync(ArtistAlbum album)
        {
            await _albumRepository.AddAsync(album);
        }

        public async Task UpdateAsync(ArtistAlbum album)
        {
            await _albumRepository.UpdateAsync(album);
        }

        public async Task DeleteAsync(int id)
        {
            await _albumRepository.DeleteAsync(id);
        }
    }
}