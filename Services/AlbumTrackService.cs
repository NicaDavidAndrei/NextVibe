using NextVibe.Models;
using NextVibe.Repositories.Interfaces;
using NextVibe.Services.Interfaces;

namespace NextVibe.Services
{
    public class AlbumTrackService : IAlbumTrackService
    {
        private readonly IAlbumTrackRepository _albumTrackRepository;

        public AlbumTrackService(IAlbumTrackRepository albumTrackRepository)
        {
            _albumTrackRepository = albumTrackRepository;
        }

        public async Task<List<AlbumTrack>> GetAllAsync()
        {
            return await _albumTrackRepository.GetAllAsync();
        }

        public async Task<AlbumTrack?> GetByIdAsync(int id)
        {
            return await _albumTrackRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(AlbumTrack track)
        {
            await _albumTrackRepository.AddAsync(track);
        }

        public async Task UpdateAsync(AlbumTrack track)
        {
            await _albumTrackRepository.UpdateAsync(track);
        }

        public async Task DeleteAsync(int id)
        {
            await _albumTrackRepository.DeleteAsync(id);
        }
    }
}