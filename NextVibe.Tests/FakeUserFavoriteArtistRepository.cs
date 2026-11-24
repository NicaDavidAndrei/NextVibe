using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NextVibe.Models;
using NextVibe.Repositories.Interfaces;

namespace NextVibe.Tests;

public class FakeUserFavoriteArtistRepository : IUserFavoriteArtistRepository
{
    private readonly List<UserFavoriteArtist> _favorites = new();

    public void SeedFavorites(IEnumerable<UserFavoriteArtist> favorites) => _favorites.AddRange(favorites);

    public Task<List<UserFavoriteArtist>> GetFavoritesByUserIdAsync(string userId)
    {
        var result = _favorites.Where(f => f.UserId == userId).ToList();
        return Task.FromResult(result);
    }

    public Task<List<UserFavoriteArtist>> GetAllWithDetailsAsync() => Task.FromResult(_favorites.ToList());

    public Task<UserFavoriteArtist?> GetFavoriteAsync(string userId, int artistId)
    {
        var fav = _favorites.FirstOrDefault(f => f.UserId == userId && f.ArtistId == artistId);
        return Task.FromResult<UserFavoriteArtist?>(fav);
    }

    public Task<bool> FavoriteExistsAsync(string userId, int artistId)
    {
        var exists = _favorites.Any(f => f.UserId == userId && f.ArtistId == artistId);
        return Task.FromResult(exists);
    }

    public Task<int> CountByUserIdAsync(string userId)
    {
        var count = _favorites.Count(f => f.UserId == userId);
        return Task.FromResult(count);
    }

    public Task<List<int>> GetFavoriteArtistIdsAsync(string userId)
    {
        var ids = _favorites.Where(f => f.UserId == userId).Select(f => f.ArtistId).ToList();
        return Task.FromResult(ids);
    }

    // IGenericRepository implementation (required by inheritance)
    public Task<List<UserFavoriteArtist>> GetAllAsync() => Task.FromResult(_favorites.ToList());
    public Task<UserFavoriteArtist?> GetByIdAsync(int id) => Task.FromResult(_favorites.FirstOrDefault(f => f.Id == id));
    public Task AddAsync(UserFavoriteArtist entity)
    {
        // Simulate auto-increment Id
        if (entity.Id == 0)
            entity.Id = _favorites.Any() ? _favorites.Max(f => f.Id) + 1 : 1;
        _favorites.Add(entity);
        return Task.CompletedTask;
    }
    public Task UpdateAsync(UserFavoriteArtist entity) => Task.CompletedTask;
    public Task DeleteAsync(int id)
    {
        var toRemove = _favorites.FirstOrDefault(f => f.Id == id);
        if (toRemove != null) _favorites.Remove(toRemove);
        return Task.CompletedTask;
    }
    public Task<bool> ExistsAsync(int id) => Task.FromResult(_favorites.Any(f => f.Id == id));
}
