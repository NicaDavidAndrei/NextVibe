using System;
using System.ComponentModel.DataAnnotations;

namespace NextVibe.Models
{
    public class GameScore
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        [MaxLength(50)]
        public string GameType { get; set; } = string.Empty; // "GuessSong", "GuessAlbumBlur", "GuessAlbumColor", "GuessAlbumRandom"

        public int Score { get; set; }

        public int CorrectAnswers { get; set; }

        public int TotalRounds { get; set; }

        public int DurationSeconds { get; set; }

        public string? TargetArtistName { get; set; }

        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    }
}

