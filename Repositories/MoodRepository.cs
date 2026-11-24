using Microsoft.EntityFrameworkCore;
using NextVibe.Data;
using NextVibe.Models;
using NextVibe.Repositories.Interfaces;

namespace NextVibe.Repositories
{
    public class MoodRepository : GenericRepository<Mood>, IMoodRepository
    {
        public MoodRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<List<Mood>> GetAllAsync()
        {
            return await _context.Moods
                .OrderBy(m => m.Name)
                .ToListAsync();
        }
    }
}