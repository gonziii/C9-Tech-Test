using Domain;

namespace SpotifyDataRepository.Service
{
    public interface ISpotifyRepositoryService
    {
        Task AddSong(SpotifySong song);
    }
}
