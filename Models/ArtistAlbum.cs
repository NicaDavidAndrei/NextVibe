using System.ComponentModel.DataAnnotations;

namespace NextVibe.Models
{
    public class ArtistAlbum
    {
        public int Id { get; set; }

        [Required]
        public int ArtistId { get; set; }

        public Artist? Artist { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Release Year")]
        public int? ReleaseYear { get; set; }

        public string? Description { get; set; }

        [Display(Name = "Cover Image")]
        public string? CoverImageUrl { get; set; }

        public string? DominantColorsJson { get; set; }

        public ICollection<AlbumTrack> Tracks { get; set; } = new List<AlbumTrack>();
    }
}