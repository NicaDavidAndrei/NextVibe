using NextVibe.Models;
using NextVibe.Repositories.Interfaces;
using NextVibe.Services.Interfaces;

namespace NextVibe.Services
{
    public class MoodService : IMoodService
    {
        private readonly IMoodRepository _moodRepository;

        public MoodService(IMoodRepository moodRepository)
        {
            _moodRepository = moodRepository;
        }

        public async Task<List<Mood>> GetAllAsync()
        {
            return await _moodRepository.GetAllAsync();
        }

        public async Task<Mood?> GetByIdAsync(int id)
        {
            return await _moodRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(Mood mood)
        {
            await _moodRepository.AddAsync(mood);
        }

        public async Task UpdateAsync(Mood mood)
        {
            await _moodRepository.UpdateAsync(mood);
        }

        public async Task DeleteAsync(int id)
        {
            await _moodRepository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _moodRepository.ExistsAsync(id);
        }
    }
}