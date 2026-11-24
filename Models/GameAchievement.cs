using System;
using System.ComponentModel.DataAnnotations;

namespace NextVibe.Models
{
    public class GameAchievement
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Icon { get; set; } = "🏆";

        public int XpReward { get; set; } = 50;
    }

    public class UserAchievement
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int AchievementId { get; set; }
        public GameAchievement? Achievement { get; set; }

        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }
}

