namespace Domain.Interfaces
{
    public interface ISpotifyRepository : IGenericRepository<SpotifySong>
    {
        void AddSong(SpotifySong song);
        void RemoveSong(SpotifySong song);
        List<SpotifySong> GetSongs();
    }
}
