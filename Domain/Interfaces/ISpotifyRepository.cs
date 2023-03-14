namespace Domain.Interfaces
{
    public interface ISpotifyRepository : IGenericRepository<SpotifySong>
    {
        SpotifySong GetSong(string name);
        void AddSong(SpotifySong song);
        void RemoveSong(SpotifySong song);
    }
}
