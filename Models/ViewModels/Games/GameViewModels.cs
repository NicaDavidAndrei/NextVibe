using System;
using System.Collections.Generic;
using NextVibe.Models;

namespace NextVibe.Models.ViewModels.Games
{
    public class GameDashboardViewModel
    {
        public int TotalGamesPlayed { get; set; }
        public int TotalScore { get; set; }
        public int BestScoreSong { get; set; }
        public int BestScoreAlbum { get; set; }
        public int BestScoreLyrics { get; set; }
        public bool HasPlayedDailyToday { get; set; }
        public int? TodayDailyScore { get; set; }
        public List<GameScore> RecentScores { get; set; } = new();
        public List<Artist> UserFavoriteArtists { get; set; } = new();
        public List<Artist> AvailableArtists { get; set; } = new();
        public bool IsAuthenticated { get; set; }
    }

    public class GuessSongRoundDto
    {
        public int RoundNumber { get; set; }
        public int TotalRounds { get; set; }
        public int TrackId { get; set; }
        public string AudioPreviewUrl { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string AlbumTitle { get; set; } = string.Empty;
        public int? ReleaseYear { get; set; }
        public string CorrectTitle { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
    }

    public class GuessAlbumRoundDto
    {
        public int RoundNumber { get; set; }
        public int TotalRounds { get; set; }
        public int AlbumId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public string Mode { get; set; } = "blur"; // "blur" or "color"
        public string? CoverImageUrl { get; set; }
        public List<string> DominantColors { get; set; } = new();
        public int? ReleaseYear { get; set; }
        public string CorrectTitle { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
    }

    public class FinishLyricRoundDto
    {
        public int RoundNumber { get; set; }
        public int TotalRounds { get; set; }
        public int ChallengeId { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string LyricsBefore { get; set; } = string.Empty;
        // Restul versului de completat.
        public string MissingWord { get; set; } = string.Empty;
        public string? Hint { get; set; }
        public string Difficulty { get; set; } = "Easy"; // "Easy" or "Hard"
        public List<string> Options { get; set; } = new();
    }

    public class UserGameStatsViewModel
    {
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TotalGames { get; set; }
        public int TotalScore { get; set; }
        public int TotalCorrectAnswers { get; set; }
        public int TotalQuestionsAttempted { get; set; }
        public double AccuracyPercentage => TotalQuestionsAttempted > 0
            ? Math.Round((double)TotalCorrectAnswers / TotalQuestionsAttempted * 100, 1)
            : 0;

        public int BestScoreSong { get; set; }
        public int BestScoreAlbum { get; set; }
        public int BestScoreLyrics { get; set; }
        public int BestScoreDaily { get; set; }

        public int CurrentStreak { get; set; }
        public string RankTitle { get; set; } = "Music Rookie";
        public string RankBadge { get; set; } = "🌱";

        public bool HasPlayedDailyToday { get; set; }
        public int? TodayDailyScore { get; set; }

        public List<GameScore> ScoreHistory { get; set; } = new();
    }

    public class SubmitGameScoreDto
    {
        public string GameType { get; set; } = string.Empty;
        public int Score { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalRounds { get; set; }
        public int DurationSeconds { get; set; }
        public string? TargetArtistName { get; set; }
    }

    public class SongSearchResultDto
    {
        public string Title { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string AlbumTitle { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
    }
}
