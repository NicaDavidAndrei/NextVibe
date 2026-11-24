using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NextVibe.Models
{
    public class Artist
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }
        public string? Country { get; set; }

        [Display(Name = "Birth Year")]
        public int? BirthYear { get; set; }

        public int GenreId { get; set; }
        public Genre? Genre { get; set; }

        public ICollection<UserFavoriteArtist>? FavoritedBy { get; set; }
        public ICollection<ArtistMood>? ArtistMoods { get; set; }
        public ICollection<ArtistAlbum> Albums { get; set; } = new List<ArtistAlbum>();
    }
}