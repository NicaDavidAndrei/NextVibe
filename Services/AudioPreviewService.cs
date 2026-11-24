using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NextVibe.Services.Interfaces;

namespace NextVibe.Services
{
    public class AudioPreviewService : IAudioPreviewService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AudioPreviewService> _logger;

        public AudioPreviewService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ILogger<AudioPreviewService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _logger = logger;
        }

        public async Task<string?> GetTrackPreviewUrlAsync(string artistName, string trackName)
        {
            if (string.IsNullOrWhiteSpace(artistName) || string.IsNullOrWhiteSpace(trackName))
            {
                return null;
            }

            var cacheKey = $"itunes_preview_v2_{Compact(artistName)}_{Compact(trackName)}";

            if (_cache.TryGetValue(cacheKey, out string? cachedUrl))
            {
                return cachedUrl;
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(5);

                var query = Uri.EscapeDataString($"{artistName} {trackName}");
                var uri = $"https://itunes.apple.com/search?term={query}&entity=song&limit=25";

                var response = await client.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                using var stream = await response.Content.ReadAsStreamAsync();
                using var jsonDoc = await JsonDocument.ParseAsync(stream);

                var root = jsonDoc.RootElement;
                if (root.TryGetProperty("results", out var results))
                {
                    foreach (var result in results.EnumerateArray())
                    {
                        if (!result.TryGetProperty("trackName", out var trackProp) ||
                            !result.TryGetProperty("artistName", out var artistProp) ||
                            !result.TryGetProperty("previewUrl", out var previewProp))
                        {
                            continue;
                        }

                        var foundTrack = trackProp.GetString() ?? string.Empty;
                        var foundArtist = artistProp.GetString() ?? string.Empty;
                        var previewUrl = previewProp.GetString();

                        if (string.IsNullOrEmpty(previewUrl) ||
                            !TitlesMatch(trackName, foundTrack) ||
                            !ArtistsMatch(artistName, foundArtist))
                        {
                            continue;
                        }

                        _cache.Set(cacheKey, previewUrl, TimeSpan.FromHours(24));
                        return previewUrl;
                    }
                }

                _logger.LogInformation(
                    "No matching iTunes preview for {Artist} - {Track}. The first search hit was a different song.",
                    artistName,
                    trackName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to query audio preview for {Artist} - {Track}", artistName, trackName);
            }

            return null;
        }

        private static string Compact(string value)
        {
            var chars = value.ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray();
            return new string(chars);
        }

        private static string CoreTitle(string name)
        {
            var cut = name;
            var paren = cut.IndexOf('(');
            if (paren > 0)
            {
                cut = cut[..paren];
            }

            var dash = cut.IndexOf(" - ", StringComparison.Ordinal);
            if (dash > 0)
            {
                cut = cut[..dash];
            }

            return Compact(cut);
        }

        private static bool TitlesMatch(string wanted, string found)
        {
            var wantedTitle = CoreTitle(wanted);
            var foundTitle = CoreTitle(found);
            return wantedTitle.Length > 0 && wantedTitle == foundTitle;
        }

        private static bool ArtistsMatch(string wanted, string found)
        {
            var wantedArtist = Compact(wanted);
            var foundArtist = Compact(found);
            if (wantedArtist.Length == 0 || foundArtist.Length == 0)
            {
                return false;
            }

            return foundArtist.Contains(wantedArtist) || wantedArtist.Contains(foundArtist);
        }
    }
}

