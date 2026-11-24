using NextVibe.Models;
using NextVibe.Repositories.Interfaces;
using NextVibe.Services.Interfaces;

namespace NextVibe.Services
{
    public class ArtistMoodService : IArtistMoodService
    {
        private readonly IArtistMoodRepository _artistMoodRepository;

        public ArtistMoodService(IArtistMoodRepository artistMoodRepository)
        {
            _artistMoodRepository = artistMoodRepository;
        }

        public async Task<List<ArtistMood>> GetAllAsync()
        {
            return await _artistMoodRepository.GetAllAsync();
        }

        public async Task<ArtistMood?> GetByIdAsync(int id)
        {
            return await _artistMoodRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(ArtistMood artistMood)
        {
            var exists = await _artistMoodRepository.ExistsAsync(
                artistMood.ArtistId,
                artistMood.MoodId);

            if (exists)
            {
                return false;
            }

            await _artistMoodRepository.AddAsync(artistMood);
            return true;
        }

        public async Task<bool> UpdateAsync(ArtistMood artistMood)
        {
            var existing = await _artistMoodRepository.GetByIdAsync(artistMood.Id);

            if (existing == null)
            {
                return false;
            }

            var duplicate = await _artistMoodRepository.ExistsAsync(
                artistMood.ArtistId,
                artistMood.MoodId);

            if (duplicate &&
                (existing.ArtistId != artistMood.ArtistId ||
                 existing.MoodId != artistMood.MoodId))
            {
                return false;
            }

            await _artistMoodRepository.UpdateAsync(artistMood);
            return true;
        }

        public async Task DeleteAsync(int id)
        {
            await _artistMoodRepository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _artistMoodRepository.ExistsAsync(id);
        }
    }
}