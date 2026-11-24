using System.ComponentModel.DataAnnotations;

namespace NextVibe.Models
{
    public class AlbumTrack
    {
        public int Id { get; set; }

        [Required]
        public int ArtistAlbumId { get; set; }

        public ArtistAlbum? ArtistAlbum { get; set; }

        [Required]
        [Display(Name = "Track Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Track Number")]
        public int TrackNumber { get; set; }

        [Display(Name = "Duration")]
        public int? DurationSeconds { get; set; }

        [Display(Name = "Is Top Track")]
        public bool IsTopTrack { get; set; }
    }
}