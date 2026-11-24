using System.Collections.Generic;

namespace NextVibe.Models
{
    public class Genre
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Artist>? Artists { get; set; }
    }
}