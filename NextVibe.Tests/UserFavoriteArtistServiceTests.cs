using System.Threading.Tasks;
using NextVibe.Models;
using NextVibe.Services;
using Xunit;

namespace NextVibe.Tests;

public class UserFavoriteArtistServiceTests
{
    private readonly FakeArtistRepository _artistRepo;
    private readonly FakeUserFavoriteArtistRepository _favoriteRepo;
    private readonly UserFavoriteArtistService _service;

    public UserFavoriteArtistServiceTests()
    {
        _artistRepo = new FakeArtistRepository();
        _favoriteRepo = new FakeUserFavoriteArtistRepository();
        _service = new UserFavoriteArtistService(_favoriteRepo, _artistRepo);
    }

    [Fact]
    public async Task DoesNotAddFavoriteWhenArtistDoesNotExist()
    {
        // No artists seeded
        await _service.AddFavoriteAsync("user1", 999);
        var favorites = await _favoriteRepo.GetFavoritesByUserIdAsync("user1");
        Assert.Empty(favorites);
    }

    [Fact]
    public async Task AddsFavoriteWhenArtistExistsAndNotAlreadyFavorited()
    {
        var artist = new Artist { Id = 1, Name = "Artist1", GenreId = 1 };
        _artistRepo.SeedArtists(new[] { artist });
        await _service.AddFavoriteAsync("user1", 1);
        var favs = await _favoriteRepo.GetFavoritesByUserIdAsync("user1");
        Assert.Single(favs);
        Assert.Equal(1, favs[0].ArtistId);
    }

    [Fact]
    public async Task DoesNotDuplicateFavoriteForSameUserAndArtist()
    {
        var artist = new Artist { Id = 1, Name = "A", GenreId = 1 };
        _artistRepo.SeedArtists(new[] { artist });
        await _service.AddFavoriteAsync("u", 1);
        await _service.AddFavoriteAsync("u", 1); // second attempt
        var favs = await _favoriteRepo.GetFavoritesByUserIdAsync("u");
        Assert.Single(favs);
    }

    [Fact]
    public async Task RemovesFavoriteWhenItBelongsToRequestingUser()
    {
        var artist = new Artist { Id = 1, Name = "A", GenreId = 1 };
        _artistRepo.SeedArtists(new[] { artist });
        await _service.AddFavoriteAsync("u", 1);
        var fav = (await _favoriteRepo.GetFavoritesByUserIdAsync("u"))[0];
        await _service.RemoveFavoriteAsync("u", fav.Id);
        var after = await _favoriteRepo.GetFavoritesByUserIdAsync("u");
        Assert.Empty(after);
    }

    [Fact]
    public async Task DoesNotRemoveFavouriteWhenBelongsToAnotherUser()
    {
        var artist = new Artist { Id = 1, Name = "A", GenreId = 1 };
        _artistRepo.SeedArtists(new[] { artist });
        await _service.AddFavoriteAsync("owner", 1);
        var fav = (await _favoriteRepo.GetFavoritesByUserIdAsync("owner"))[0];
        await _service.RemoveFavoriteAsync("other", fav.Id);
        var after = await _favoriteRepo.GetFavoritesByUserIdAsync("owner");
        Assert.Single(after);
    }

    [Fact]
    public async Task DoesNotFailWhenRemovingNonexistentFavourite()
    {
        // Attempt removal of non-existent favorite id
        await _service.RemoveFavoriteAsync("u", 999);
        // No exception expected, simply passes.
        Assert.True(true);
    }
}
