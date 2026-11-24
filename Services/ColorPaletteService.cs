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
using NextVibe.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace NextVibe.Services
{
    public class ColorPaletteService : IColorPaletteService
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ColorPaletteService> _logger;

        public ColorPaletteService(
            AppDbContext db,
            IWebHostEnvironment env,
            ILogger<ColorPaletteService> logger)
        {
            _db = db;
            _env = env;
            _logger = logger;
        }

        public List<string> ExtractDominantColors(string imageFilePath, int count = 4)
        {
            var fallback = new List<string> { "#6d5efc", "#ff5964", "#35a7ff", "#ffc857" };

            if (!File.Exists(imageFilePath))
            {
                return fallback;
            }

            try
            {
                using var image = Image.Load<Rgba32>(imageFilePath);

                // Resize to small thumbnail for fast and smooth color extraction
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(48, 48),
                    Sampler = KnownResamplers.Bicubic
                }));

                // Bucket pixels into a quantized 4x4x4 or 8x8x8 space (step of 32)
                var buckets = new Dictionary<string, (int R, int G, int B, int Count)>();

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        var pixel = image[x, y];
                        if (pixel.A < 128) continue; // ignore transparent pixels

                        // Quantize to step of 32 to cluster nearby shades
                        int qR = (pixel.R / 32) * 32 + 16;
                        int qG = (pixel.G / 32) * 32 + 16;
                        int qB = (pixel.B / 32) * 32 + 16;

                        string key = $"{qR}_{qG}_{qB}";
                        if (buckets.TryGetValue(key, out var val))
                        {
                            buckets[key] = (val.R, val.G, val.B, val.Count + 1);
                        }
                        else
                        {
                            buckets[key] = (qR, qG, qB, 1);
                        }
                    }
                }

                // Sort buckets by pixel frequency
                var sorted = buckets.Values.OrderByDescending(b => b.Count).ToList();

                var chosenColors = new List<(int R, int G, int B)>();

                // Select distinct colors with a minimum perceptual color distance
                foreach (var b in sorted)
                {
                    bool isDistinct = true;
                    foreach (var existing in chosenColors)
                    {
                        int distSq = (b.R - existing.R) * (b.R - existing.R) +
                                     (b.G - existing.G) * (b.G - existing.G) +
                                     (b.B - existing.B) * (b.B - existing.B);

                        // Distance threshold ~ 45
                        if (distSq < 45 * 45)
                        {
                            isDistinct = false;
                            break;
                        }
                    }

                    if (isDistinct)
                    {
                        chosenColors.Add((b.R, b.G, b.B));
                        if (chosenColors.Count >= count)
                        {
                            break;
                        }
                    }
                }

                // If not enough distinct colors, fill from sorted list
                if (chosenColors.Count < count)
                {
                    foreach (var b in sorted)
                    {
                        if (!chosenColors.Any(c => c.R == b.R && c.G == b.G && c.B == b.B))
                        {
                            chosenColors.Add((b.R, b.G, b.B));
                            if (chosenColors.Count >= count) break;
                        }
                    }
                }

                if (chosenColors.Count == 0)
                {
                    return fallback;
                }

                return chosenColors
                    .Select(c => $"#{c.R:X2}{c.G:X2}{c.B:X2}")
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract dominant colors from {Path}", imageFilePath);
                return fallback;
            }
        }

        public async Task EnsureAlbumColorsPopulatedAsync()
        {
            var albumsMissingColors = await _db.ArtistAlbums
                .Where(a => string.IsNullOrEmpty(a.DominantColorsJson) && !string.IsNullOrEmpty(a.CoverImageUrl))
                .ToListAsync();

            if (!albumsMissingColors.Any())
            {
                return;
            }

            bool updatedAny = false;

            foreach (var album in albumsMissingColors)
            {
                var relative = album.CoverImageUrl!.TrimStart('/', '\\');
                var fullPath = Path.Combine(_env.WebRootPath, relative);

                if (File.Exists(fullPath))
                {
                    var colors = ExtractDominantColors(fullPath, 4);
                    album.DominantColorsJson = JsonSerializer.Serialize(colors);
                    updatedAny = true;
                }
            }

            if (updatedAny)
            {
                await _db.SaveChangesAsync();
                _logger.LogInformation("Successfully populated dominant colors for {Count} albums.", albumsMissingColors.Count);
            }
        }
    }
}

