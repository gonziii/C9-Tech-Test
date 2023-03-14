using Domain;

namespace TechTestBackend.Services
{
    public interface ISpotifyService
    {
        Task<List<SpotifySong>?> GetTracks(string name);
        Task<SpotifySong?> GetTrack(string id);
    }
}
