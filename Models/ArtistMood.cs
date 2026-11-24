namespace NextVibe.Models
{
    public class ArtistMood
    {
        public int Id { get; set; }

        public int ArtistId { get; set; }
        public Artist? Artist { get; set; }

        public int MoodId { get; set; }
        public Mood? Mood { get; set; }
    }
}