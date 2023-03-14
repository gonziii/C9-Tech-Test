using Domain;
using Domain.Interfaces;

namespace SpotifyDataRepository.Repositories
{
    public class SpotifyRepository : GenericRepository<SpotifySong>, ISpotifyRepository
    {
        public SpotifyRepository(SongStorageContext context) : base(context)
        {
        }

        public SpotifySong GetSong(string name)
        {
            return _context.Songs.SingleOrDefault(x => x.Name == name);
        }

        public void AddSong(SpotifySong song)
        {
            _context.Songs.Add(song);
        }

        public void RemoveSong(SpotifySong song)
        {
            _context.Remove(song);
        }
    }
}
