using Domain;
using Domain.Interfaces;

namespace SpotifyDataRepository.Repositories
{
    public class SpotifyRepository : GenericRepository<SpotifySong>, ISpotifyRepository
    {
        public SpotifyRepository(SongStorageContext context) : base(context)
        {
        }

        public void AddSong(SpotifySong song)
        {
            _context.Songs.Add(song);
        }

        public void RemoveSong(SpotifySong song)
        {
            _context.Remove(song);
        }

        public List<SpotifySong> GetSongs()
        {
            return _context.Songs.ToList();
        }
    }
}
