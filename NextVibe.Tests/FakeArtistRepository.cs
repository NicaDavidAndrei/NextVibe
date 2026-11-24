using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NextVibe.Models;
using NextVibe.Repositories.Interfaces;

namespace NextVibe.Tests;

public class FakeArtistRepository : IArtistRepository
{
    private readonly List<Artist> _artists = new();

    public void SeedArtists(IEnumerable<Artist> artists) => _artists.AddRange(artists);

    public Task<List<Artist>> SearchAsync(string? searchString, int? genreId, int? moodId)
    {
        // Simple filter for tests; not used in current scenarios.
        var result = _artists.AsQueryable();
        if (!string.IsNullOrEmpty(searchString))
            result = result.Where(a => a.Name.Contains(searchString));
        if (genreId.HasValue)
            result = result.Where(a => a.GenreId == genreId.Value);
        // Mood filtering omitted for brevity.
        return Task.FromResult(result.ToList());
    }

    public Task<List<Artist>> GetRecommendationsByFavoriteGenresAsync(List<int> favoriteGenreIds, List<int> favoriteArtistIds)
    {
        // Return artists whose GenreId is in favoriteGenreIds and not already a favorite.
        var recommendations = _artists
            .Where(a => favoriteGenreIds.Contains(a.GenreId) && !favoriteArtistIds.Contains(a.Id))
            .ToList();
        return Task.FromResult(recommendations);
    }

    // IGenericRepository implementation (required by interface inheritance)
    public Task<List<Artist>> GetAllAsync() => Task.FromResult(_artists.ToList());
    public Task<Artist?> GetByIdAsync(int id) => Task.FromResult(_artists.FirstOrDefault(a => a.Id == id));
    public Task AddAsync(Artist entity)
    {
        _artists.Add(entity);
        return Task.CompletedTask;
    }
    public Task UpdateAsync(Artist entity) => Task.CompletedTask;
    public Task DeleteAsync(int id) => Task.CompletedTask;
    public Task<bool> ExistsAsync(int id) => Task.FromResult(_artists.Any(a => a.Id == id));
}
