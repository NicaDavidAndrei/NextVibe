using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextVibe.Services.Interfaces
{
    public interface IColorPaletteService
    {
        List<string> ExtractDominantColors(string imageFilePath, int count = 4);
        Task EnsureAlbumColorsPopulatedAsync();
    }
}

