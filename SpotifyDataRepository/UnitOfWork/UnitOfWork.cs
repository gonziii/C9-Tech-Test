using Domain.Interfaces;

namespace SpotifyDataRepository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SongStorageContext _context;
        public UnitOfWork(SongStorageContext context)
        {
            _context = context;
            Spotify = new Repositories.SpotifyRepository(_context);
        }
        public ISpotifyRepository Spotify { get; private set; }
        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
