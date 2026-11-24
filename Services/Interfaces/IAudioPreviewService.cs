using System.Threading.Tasks;

namespace NextVibe.Services.Interfaces
{
    public interface IAudioPreviewService
    {
        Task<string?> GetTrackPreviewUrlAsync(string artistName, string trackName);
    }
}

