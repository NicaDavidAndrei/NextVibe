using System.Collections.Generic;
using System.Threading.Tasks;
using NextVibe.Models;
using NextVibe.Models.ViewModels.Games;

namespace NextVibe.Services.Interfaces
{
    public interface IGamesService
    {
        Task<GameDashboardViewModel> GetDashboardDataAsync(string? userId);
        Task<List<GuessSongRoundDto>> GenerateGuessSongRoundsAsync(int? artistId, string? userId, int roundCount = 5);
        Task<List<GuessAlbumRoundDto>> GenerateGuessAlbumRoundsAsync(string mode, int? artistId, string? userId, int roundCount = 5);
        Task<List<FinishLyricRoundDto>> GenerateFinishLyricRoundsAsync(string difficulty, int? artistId, int roundCount = 5);
        Task<List<string>> GetAllSongTitlesAsync(int? artistId = null);
        Task<List<SongSearchResultDto>> SearchSongsAsync(string? query, int? artistId = null);
        Task<List<GuessSongRoundDto>> GenerateDailyChallengeRoundsAsync();
        Task<bool> HasPlayedDailyTodayAsync(string userId);
        Task<UserGameStatsViewModel> GetUserGameStatsAsync(string userId);
        Task<GameScore> SaveGameScoreAsync(string? userId, SubmitGameScoreDto dto);
        Task<List<GameScore>> GetLeaderboardAsync(string? gameType = null, int limit = 10);
    }
}
