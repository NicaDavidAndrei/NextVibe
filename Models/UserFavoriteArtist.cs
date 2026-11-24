using System;

namespace NextVibe.Models
{
    public class UserFavoriteArtist
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int ArtistId { get; set; }
        public Artist? Artist { get; set; }

        public DateTime AddedAt { get; set; }
    }
}