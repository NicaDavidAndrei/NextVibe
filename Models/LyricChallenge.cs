using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NextVibe.Models
{
    public class LyricChallenge
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string TrackName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ArtistName { get; set; } = string.Empty;

        [Required]
        public string LyricsBefore { get; set; } = string.Empty;

        // Restul versului, nu un singur cuvânt: "work is never over"
        [Required]
        [MaxLength(100)]
        public string MissingWord { get; set; } = string.Empty;

        // JSON array of 3 wrong line endings: ["line one", "line two", "line three"]
        public string DistractorsJson { get; set; } = "[]";

        [MaxLength(200)]
        public string? Hint { get; set; }

        [MaxLength(20)]
        public string Difficulty { get; set; } = "Medium"; // "Easy", "Medium", "Hard"
    }
}

