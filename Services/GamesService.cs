using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NextVibe.Data;
using NextVibe.Models;
using NextVibe.Models.ViewModels.Games;
using NextVibe.Services.Interfaces;

namespace NextVibe.Services
{
    public class GamesService : IGamesService
    {
        private readonly AppDbContext _db;
        private readonly IAudioPreviewService _audioPreviewService;
        private readonly IColorPaletteService _colorPaletteService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GamesService> _logger;

        public GamesService(
            AppDbContext db,
            IAudioPreviewService audioPreviewService,
            IColorPaletteService colorPaletteService,
            IWebHostEnvironment env,
            ILogger<GamesService> logger)
        {
            _db = db;
            _audioPreviewService = audioPreviewService;
            _colorPaletteService = colorPaletteService;
            _env = env;
            _logger = logger;
        }

        public async Task<GameDashboardViewModel> GetDashboardDataAsync(string? userId)
        {
            var vm = new GameDashboardViewModel
            {
                IsAuthenticated = !string.IsNullOrEmpty(userId)
            };

            // Get available artists that have albums & tracks
            vm.AvailableArtists = await _db.Artists
                .Where(a => a.Albums.Any(alb => alb.Tracks.Any()))
                .OrderBy(a => a.Name)
                .ToListAsync();

            if (!string.IsNullOrEmpty(userId))
            {
                vm.UserFavoriteArtists = await _db.UserFavoriteArtists
                    .Where(f => f.UserId == userId)
                    .Include(f => f.Artist)
                    .Select(f => f.Artist!)
                    .ToListAsync();

                var userScores = await _db.GameScores
                    .Where(s => s.UserId == userId)
                    .OrderByDescending(s => s.PlayedAt)
                    .ToListAsync();

                vm.TotalGamesPlayed = userScores.Count;
                vm.TotalScore = userScores.Sum(s => s.Score);

                vm.BestScoreSong = userScores
                    .Where(s => s.GameType == "GuessSong")
                    .Select(s => (int?)s.Score)
                    .Max() ?? 0;

                vm.BestScoreAlbum = userScores
                    .Where(s => s.GameType.StartsWith("GuessAlbum"))
                    .Select(s => (int?)s.Score)
                    .Max() ?? 0;

                vm.BestScoreLyrics = userScores
                    .Where(s => s.GameType == "FinishLyric")
                    .Select(s => (int?)s.Score)
                    .Max() ?? 0;

                var todayDaily = userScores.FirstOrDefault(s => s.GameType == "DailyChallenge" && s.PlayedAt.Date == DateTime.UtcNow.Date);
                vm.HasPlayedDailyToday = todayDaily != null;
                vm.TodayDailyScore = todayDaily?.Score;

                vm.RecentScores = userScores.Take(5).ToList();
            }
            else
            {
                vm.RecentScores = await _db.GameScores
                    .Include(s => s.User)
                    .OrderByDescending(s => s.PlayedAt)
                    .Take(5)
                    .ToListAsync();
            }

            return vm;
        }

        public async Task<List<GuessSongRoundDto>> GenerateGuessSongRoundsAsync(int? artistId, string? userId, int roundCount = 5)
        {
            var query = _db.AlbumTracks
                .Include(t => t.ArtistAlbum)
                .ThenInclude(a => a!.Artist)
                .AsQueryable();

            if (artistId.HasValue && artistId.Value > 0)
            {
                query = query.Where(t => t.ArtistAlbum != null && t.ArtistAlbum.ArtistId == artistId.Value);
            }
            else if (artistId.HasValue && artistId.Value == -1 && !string.IsNullOrEmpty(userId))
            {
                var favArtistIds = await _db.UserFavoriteArtists
                    .Where(f => f.UserId == userId)
                    .Select(f => f.ArtistId)
                    .ToListAsync();

                if (favArtistIds.Any())
                {
                    query = query.Where(t => t.ArtistAlbum != null && favArtistIds.Contains(t.ArtistAlbum.ArtistId));
                }
            }

            var allCandidateTracks = await query.ToListAsync();

            if (allCandidateTracks.Count < roundCount)
            {
                allCandidateTracks = await _db.AlbumTracks
                    .Include(t => t.ArtistAlbum)
                    .ThenInclude(a => a!.Artist)
                    .ToListAsync();
            }

            var allTitles = await _db.AlbumTracks.Select(t => t.Name).Distinct().ToListAsync();
            var shuffledCandidates = allCandidateTracks.OrderBy(_ => Random.Shared.Next()).ToList();
            var rounds = new List<GuessSongRoundDto>();

            foreach (var track in shuffledCandidates)
            {
                if (track.ArtistAlbum?.Artist == null) continue;

                var artistName = track.ArtistAlbum.Artist.Name;
                var previewUrl = await _audioPreviewService.GetTrackPreviewUrlAsync(artistName, track.Name);

                if (string.IsNullOrEmpty(previewUrl))
                {
                    continue;
                }

                var distractors = allTitles
                    .Where(t => !string.Equals(t, track.Name, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(_ => Random.Shared.Next())
                    .Take(3)
                    .ToList();

                var options = new List<string>(distractors) { track.Name };
                options = options.OrderBy(_ => Random.Shared.Next()).ToList();

                rounds.Add(new GuessSongRoundDto
                {
                    RoundNumber = rounds.Count + 1,
                    TotalRounds = roundCount,
                    TrackId = track.Id,
                    AudioPreviewUrl = previewUrl,
                    ArtistName = artistName,
                    AlbumTitle = track.ArtistAlbum.Title,
                    ReleaseYear = track.ArtistAlbum.ReleaseYear,
                    CorrectTitle = track.Name,
                    Options = options
                });

                if (rounds.Count >= roundCount)
                {
                    break;
                }
            }

            return rounds;
        }

        public async Task<List<GuessAlbumRoundDto>> GenerateGuessAlbumRoundsAsync(string mode, int? artistId, string? userId, int roundCount = 5)
        {
            var query = _db.ArtistAlbums
                .Include(a => a.Artist)
                .Where(a => !string.IsNullOrEmpty(a.CoverImageUrl))
                .AsQueryable();

            if (artistId.HasValue && artistId.Value > 0)
            {
                query = query.Where(a => a.ArtistId == artistId.Value);
            }
            else if (artistId.HasValue && artistId.Value == -1 && !string.IsNullOrEmpty(userId))
            {
                var favArtistIds = await _db.UserFavoriteArtists
                    .Where(f => f.UserId == userId)
                    .Select(f => f.ArtistId)
                    .ToListAsync();

                if (favArtistIds.Any())
                {
                    query = query.Where(a => favArtistIds.Contains(a.ArtistId));
                }
            }

            var candidateAlbums = await query.ToListAsync();

            if (candidateAlbums.Count < roundCount)
            {
                candidateAlbums = await _db.ArtistAlbums
                    .Include(a => a.Artist)
                    .Where(a => !string.IsNullOrEmpty(a.CoverImageUrl))
                    .ToListAsync();
            }

            var allAlbumTitles = await _db.ArtistAlbums.Select(a => a.Title).Distinct().ToListAsync();
            var shuffledAlbums = candidateAlbums.OrderBy(_ => Random.Shared.Next()).Take(roundCount).ToList();

            var rounds = new List<GuessAlbumRoundDto>();
            int currentRound = 1;

            foreach (var album in shuffledAlbums)
            {
                string chosenMode;
                if (string.Equals(mode, "random", StringComparison.OrdinalIgnoreCase))
                {
                    chosenMode = Random.Shared.Next(2) == 0 ? "blur" : "color";
                }
                else if (string.Equals(mode, "color", StringComparison.OrdinalIgnoreCase))
                {
                    chosenMode = "color";
                }
                else
                {
                    chosenMode = "blur";
                }

                List<string> colors;
                if (!string.IsNullOrEmpty(album.DominantColorsJson))
                {
                    try
                    {
                        colors = JsonSerializer.Deserialize<List<string>>(album.DominantColorsJson) ?? new();
                    }
                    catch
                    {
                        colors = new List<string> { "#6d5efc", "#ff5964", "#35a7ff", "#ffc857" };
                    }
                }
                else
                {
                    var relative = album.CoverImageUrl!.TrimStart('/', '\\');
                    var fullPath = Path.Combine(_env.WebRootPath, relative);
                    colors = _colorPaletteService.ExtractDominantColors(fullPath, 4);

                    album.DominantColorsJson = JsonSerializer.Serialize(colors);
                    await _db.SaveChangesAsync();
                }

                var distractors = allAlbumTitles
                    .Where(t => !string.Equals(t, album.Title, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(_ => Random.Shared.Next())
                    .Take(3)
                    .ToList();

                var options = new List<string>(distractors) { album.Title };
                options = options.OrderBy(_ => Random.Shared.Next()).ToList();

                rounds.Add(new GuessAlbumRoundDto
                {
                    RoundNumber = currentRound++,
                    TotalRounds = shuffledAlbums.Count,
                    AlbumId = album.Id,
                    ArtistName = album.Artist?.Name ?? "Unknown Artist",
                    Mode = chosenMode,
                    CoverImageUrl = album.CoverImageUrl,
                    DominantColors = colors,
                    ReleaseYear = album.ReleaseYear,
                    CorrectTitle = album.Title,
                    Options = options
                });
            }

            return rounds;
        }

        public async Task<List<FinishLyricRoundDto>> GenerateFinishLyricRoundsAsync(string difficulty, int? artistId, int roundCount = 5)
        {
            var query = _db.LyricChallenges.AsQueryable();

            if (artistId.HasValue && artistId.Value > 0)
            {
                var artist = await _db.Artists.FindAsync(artistId.Value);
                if (artist != null)
                {
                    query = query.Where(l => l.ArtistName.Contains(artist.Name) || artist.Name.Contains(l.ArtistName));
                }
            }

            var candidates = await query.ToListAsync();

            if (candidates.Count < roundCount)
            {
                candidates = await _db.LyricChallenges.ToListAsync();
            }

            var selected = candidates.OrderBy(_ => Random.Shared.Next()).Take(roundCount).ToList();
            var rounds = new List<FinishLyricRoundDto>();

            int currentNum = 1;
            foreach (var item in selected)
            {
                List<string> distractors;
                try
                {
                    distractors = JsonSerializer.Deserialize<List<string>>(item.DistractorsJson) ?? new();
                }
                catch
                {
                    distractors = new List<string> { "nothing else tonight", "somewhere down the line", "only for a while" };
                }

                var options = new List<string>(distractors) { item.MissingWord };
                options = options.OrderBy(_ => Random.Shared.Next()).ToList();

                rounds.Add(new FinishLyricRoundDto
                {
                    RoundNumber = currentNum++,
                    TotalRounds = selected.Count,
                    ChallengeId = item.Id,
                    TrackName = item.TrackName,
                    ArtistName = item.ArtistName,
                    LyricsBefore = item.LyricsBefore,
                    MissingWord = item.MissingWord,
                    Hint = item.Hint,
                    Difficulty = string.IsNullOrEmpty(difficulty) ? item.Difficulty : difficulty,
                    Options = options
                });
            }

            return rounds;
        }

        public async Task<List<GuessSongRoundDto>> GenerateDailyChallengeRoundsAsync()
        {
            // Deterministic seed based on today's calendar date
            var today = DateTime.UtcNow.Date;
            int todaySeed = today.Year * 10000 + today.Month * 100 + today.Day;
            var dailyRng = new Random(todaySeed);

            var tracks = await _db.AlbumTracks
                .Include(t => t.ArtistAlbum)
                .ThenInclude(a => a!.Artist)
                .ToListAsync();

            var allTitles = tracks.Select(t => t.Name).Distinct().ToList();
            var shuffledTracks = tracks.OrderBy(_ => dailyRng.Next()).ToList();

            var rounds = new List<GuessSongRoundDto>();

            foreach (var track in shuffledTracks)
            {
                if (track.ArtistAlbum?.Artist == null) continue;

                var artistName = track.ArtistAlbum.Artist.Name;
                var previewUrl = await _audioPreviewService.GetTrackPreviewUrlAsync(artistName, track.Name);

                if (string.IsNullOrEmpty(previewUrl)) continue;

                var distractors = allTitles
                    .Where(t => !string.Equals(t, track.Name, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(_ => dailyRng.Next())
                    .Take(3)
                    .ToList();

                var options = new List<string>(distractors) { track.Name };
                options = options.OrderBy(_ => dailyRng.Next()).ToList();

                rounds.Add(new GuessSongRoundDto
                {
                    RoundNumber = rounds.Count + 1,
                    TotalRounds = 5,
                    TrackId = track.Id,
                    AudioPreviewUrl = previewUrl,
                    ArtistName = artistName,
                    AlbumTitle = track.ArtistAlbum.Title,
                    ReleaseYear = track.ArtistAlbum.ReleaseYear,
                    CorrectTitle = track.Name,
                    Options = options
                });

                if (rounds.Count >= 5) break;
            }

            return rounds;
        }

        public async Task<bool> HasPlayedDailyTodayAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return false;

            var today = DateTime.UtcNow.Date;
            return await _db.GameScores
                .AnyAsync(s => s.UserId == userId && s.GameType == "DailyChallenge" && s.PlayedAt.Date == today);
        }

        public async Task<UserGameStatsViewModel> GetUserGameStatsAsync(string userId)
        {
            var user = await _db.Users.FindAsync(userId);
            var scores = await _db.GameScores
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.PlayedAt)
                .ToListAsync();

            var vm = new UserGameStatsViewModel
            {
                IsAuthenticated = true,
                UserName = user != null
                    ? (!string.IsNullOrWhiteSpace(user.FirstName) ? $"{user.FirstName} {user.LastName}".Trim() : user.UserName ?? "Player")
                    : "Player",
                TotalGames = scores.Count,
                TotalScore = scores.Sum(s => s.Score),
                TotalCorrectAnswers = scores.Sum(s => s.CorrectAnswers),
                TotalQuestionsAttempted = scores.Sum(s => s.TotalRounds),
                ScoreHistory = scores
            };

            vm.BestScoreSong = scores.Where(s => s.GameType == "GuessSong").Select(s => (int?)s.Score).Max() ?? 0;
            vm.BestScoreAlbum = scores.Where(s => s.GameType.StartsWith("GuessAlbum")).Select(s => (int?)s.Score).Max() ?? 0;
            vm.BestScoreLyrics = scores.Where(s => s.GameType == "FinishLyric").Select(s => (int?)s.Score).Max() ?? 0;
            vm.BestScoreDaily = scores.Where(s => s.GameType == "DailyChallenge").Select(s => (int?)s.Score).Max() ?? 0;

            var today = DateTime.UtcNow.Date;
            var todayDaily = scores.FirstOrDefault(s => s.GameType == "DailyChallenge" && s.PlayedAt.Date == today);
            vm.HasPlayedDailyToday = todayDaily != null;
            vm.TodayDailyScore = todayDaily?.Score;

            // Calculate streak (consecutive days played)
            var playedDays = scores.Select(s => s.PlayedAt.Date).Distinct().OrderByDescending(d => d).ToList();
            int streak = 0;
            var checkDate = today;

            if (playedDays.Contains(checkDate))
            {
                streak++;
                checkDate = checkDate.AddDays(-1);
            }
            else if (playedDays.Contains(checkDate.AddDays(-1)))
            {
                checkDate = checkDate.AddDays(-1);
            }

            while (playedDays.Contains(checkDate))
            {
                streak++;
                checkDate = checkDate.AddDays(-1);
            }
            vm.CurrentStreak = streak;

            // Rank based on TotalScore
            if (vm.TotalScore >= 5000)
            {
                vm.RankTitle = "NextVibe Legend";
                vm.RankBadge = "👑";
            }
            else if (vm.TotalScore >= 2500)
            {
                vm.RankTitle = "Rhythm Master";
                vm.RankBadge = "🎸";
            }
            else if (vm.TotalScore >= 1200)
            {
                vm.RankTitle = "Hit Expert";
                vm.RankBadge = "⚡";
            }
            else if (vm.TotalScore >= 500)
            {
                vm.RankTitle = "Dedicated Listener";
                vm.RankBadge = "🔥";
            }
            else if (vm.TotalScore >= 200)
            {
                vm.RankTitle = "Music Fan";
                vm.RankBadge = "🎧";
            }
            else
            {
                vm.RankTitle = "Music Rookie";
                vm.RankBadge = "🌱";
            }

            return vm;
        }

        public async Task<GameScore> SaveGameScoreAsync(string? userId, SubmitGameScoreDto dto)
        {
            var gameScore = new GameScore
            {
                UserId = string.IsNullOrWhiteSpace(userId) ? null : userId,
                GameType = dto.GameType,
                Score = dto.Score,
                CorrectAnswers = dto.CorrectAnswers,
                TotalRounds = dto.TotalRounds,
                DurationSeconds = dto.DurationSeconds,
                TargetArtistName = dto.TargetArtistName,
                PlayedAt = DateTime.UtcNow
            };

            _db.GameScores.Add(gameScore);
            await _db.SaveChangesAsync();

            return gameScore;
        }

        public async Task<List<string>> GetAllSongTitlesAsync(int? artistId = null)
        {
            var query = _db.AlbumTracks.AsQueryable();
            if (artistId.HasValue && artistId > 0)
            {
                query = query.Where(t => t.ArtistAlbum != null && t.ArtistAlbum.ArtistId == artistId.Value);
            }
            return await query.Select(t => t.Name).Distinct().OrderBy(t => t).ToListAsync();
        }

        public async Task<List<SongSearchResultDto>> SearchSongsAsync(string? query, int? artistId = null)
        {
            var tracks = _db.AlbumTracks
                .Include(t => t.ArtistAlbum)
                .ThenInclude(a => a!.Artist)
                .AsQueryable();

            if (artistId.HasValue && artistId > 0)
            {
                tracks = tracks.Where(t => t.ArtistAlbum != null && t.ArtistAlbum.ArtistId == artistId.Value);
            }

            var hasQuery = !string.IsNullOrWhiteSpace(query);
            if (hasQuery)
            {
                var cleanQ = query!.Trim().ToLower();
                tracks = tracks.Where(t => t.Name.ToLower().Contains(cleanQ));
            }

            var rows = await tracks
                .Select(t => new SongSearchResultDto
                {
                    Title = t.Name,
                    ArtistName = t.ArtistAlbum != null && t.ArtistAlbum.Artist != null ? t.ArtistAlbum.Artist.Name : "Unknown",
                    AlbumTitle = t.ArtistAlbum != null ? t.ArtistAlbum.Title : "",
                    CoverImageUrl = t.ArtistAlbum != null ? t.ArtistAlbum.CoverImageUrl : null
                })
                .ToListAsync();

            return rows
                .GroupBy(t => t.Title.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.CoverImageUrl)) ?? g.First())
                .OrderBy(t => t.Title)
                .Take(hasQuery ? 12 : 1000)
                .ToList();
        }

        public async Task<List<GameScore>> GetLeaderboardAsync(string? gameType = null, int limit = 10)
        {
            var query = _db.GameScores
                .Include(s => s.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(gameType))
            {
                query = query.Where(s => s.GameType == gameType);
            }

            return await query
                .OrderByDescending(s => s.Score)
                .ThenBy(s => s.DurationSeconds)
                .Take(limit)
                .ToListAsync();
        }
    }
}
