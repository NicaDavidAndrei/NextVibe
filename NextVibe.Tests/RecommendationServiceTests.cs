using System.Collections.Generic;
using System.Threading.Tasks;
using NextVibe.Models;
using NextVibe.Services;
using NextVibe.Tests;
using Xunit;

namespace NextVibe.Tests;

public class RecommendationServiceTests
{
    private readonly FakeArtistRepository _artistRepo;
    private readonly FakeUserFavoriteArtistRepository _favoriteRepo;
    private readonly RecommendationService _service;

    public RecommendationServiceTests()
    {
        _artistRepo = new FakeArtistRepository();
        _favoriteRepo = new FakeUserFavoriteArtistRepository();
        _service = new RecommendationService(_favoriteRepo, _artistRepo);
    }

    [Fact]
    public async Task ReturnsEmptyWhenUserHasNoFavorites()
    {
        // Arrange: no favorites seeded
        var result = await _service.GetRecommendationsForUserAsync("user1");
        Assert.Empty(result);
    }

    [Fact]
    public async Task ReturnsEmptyWhenFavoritesHaveNoValidGenres()
    {
        // Arrange: favorite with artist having GenreId = 0
        var artist = new Artist { Id = 1, Name = "ArtistZero", GenreId = 0 };
        _artistRepo.SeedArtists(new[] { artist });
        _favoriteRepo.SeedFavorites(new[] { new UserFavoriteArtist { Id = 1, UserId = "user1", ArtistId = 1, Artist = artist } });

        var result = await _service.GetRecommendationsForUserAsync("user1");
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExtractsDistinctGenreIds_FromFavorites()
    {
        var artist1 = new Artist { Id = 1, Name = "A1", GenreId = 10 };
        var artist2 = new Artist { Id = 2, Name = "A2", GenreId = 20 };
        var artist3 = new Artist { Id = 3, Name = "A3", GenreId = 10 };
        _artistRepo.SeedArtists(new[] { artist1, artist2, artist3 });
        _favoriteRepo.SeedFavorites(new[] {
            new UserFavoriteArtist { Id = 1, UserId = "u", ArtistId = 1, Artist = artist1 },
            new UserFavoriteArtist { Id = 2, UserId = "u", ArtistId = 2, Artist = artist2 },
            new UserFavoriteArtist { Id = 3, UserId = "u", ArtistId = 3, Artist = artist3 }
        });
        // Act
        var result = await _service.GetRecommendationsForUserAsync("u");
        // Recommendations are based on distinct genres (10,20) and exclude favorite ids (1,2,3).
        // Since no other artists exist, result should be empty.
        Assert.Empty(result);
    }

    [Fact]
    public async Task PassesFavoriteArtistAndGenreIdsToRepository()
    {
        var artist1 = new Artist { Id = 1, Name = "A1", GenreId = 5 };
        var artist2 = new Artist { Id = 2, Name = "A2", GenreId = 6 };
        var other = new Artist { Id = 3, Name = "Other", GenreId = 5 };
        _artistRepo.SeedArtists(new[] { artist1, artist2, other });
        _favoriteRepo.SeedFavorites(new[] {
            new UserFavoriteArtist { Id = 1, UserId = "u", ArtistId = 1, Artist = artist1 },
            new UserFavoriteArtist { Id = 2, UserId = "u", ArtistId = 2, Artist = artist2 }
        });
        var result = await _service.GetRecommendationsForUserAsync("u");
        // Expect 'other' to be returned because its genre (5) is in favorites and not already a favorite.
        Assert.Single(result);
        Assert.Equal(3, result[0].Id);
    }

    [Fact]
    public async Task ReturnsRecommendationsFromRepository()
    {
        var artistFav = new Artist { Id = 1, Name = "Fav", GenreId = 1 };
        var recommendation = new Artist { Id = 2, Name = "Rec", GenreId = 1 };
        _artistRepo.SeedArtists(new[] { artistFav, recommendation });
        _favoriteRepo.SeedFavorites(new[] { new UserFavoriteArtist { Id = 1, UserId = "u", ArtistId = 1, Artist = artistFav } });
        var result = await _service.GetRecommendationsForUserAsync("u");
        Assert.Single(result);
        Assert.Equal(recommendation.Id, result[0].Id);
    }
}
