using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NextVibe.Models;

namespace NextVibe.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Mood> Moods { get; set; }
        public DbSet<UserFavoriteArtist> UserFavoriteArtists { get; set; }
        public DbSet<ArtistMood> ArtistMoods { get; set; }
        public DbSet<ArtistAlbum> ArtistAlbums { get; set; }
        public DbSet<AlbumTrack> AlbumTracks { get; set; }
        public DbSet<GameScore> GameScores { get; set; }
        public DbSet<LyricChallenge> LyricChallenges { get; set; }
        public DbSet<GameAchievement> GameAchievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
    }
}