using NextVibe.Models;
using NextVibe.Repositories.Interfaces;
using NextVibe.Services.Interfaces;

namespace NextVibe.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artistRepository;

        public ArtistService(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        public async Task<List<Artist>> GetAllAsync()
        {
            return await _artistRepository.GetAllAsync();
        }

        public async Task<Artist?> GetByIdAsync(int id)
        {
            return await _artistRepository.GetByIdAsync(id);
        }

        public async Task<List<Artist>> SearchAsync(string? searchString, int? genreId, int? moodId)
        {
            return await _artistRepository.SearchAsync(searchString, genreId, moodId);
        }

        public async Task CreateAsync(Artist artist)
        {
            await _artistRepository.AddAsync(artist);
        }

        public async Task UpdateAsync(Artist artist)
        {
            await _artistRepository.UpdateAsync(artist);
        }

        public async Task DeleteAsync(int id)
        {
            await _artistRepository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _artistRepository.ExistsAsync(id);
        }
    }
}