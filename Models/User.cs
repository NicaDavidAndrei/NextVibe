using System;
using System.Collections.Generic;

namespace NextVibe.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<UserFavoriteArtist>? Favorites { get; set; }
    }
}