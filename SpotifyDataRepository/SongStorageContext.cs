using Domain;
using Microsoft.EntityFrameworkCore;

namespace SpotifyDataRepository
{
    public class SongStorageContext : DbContext
    {
        public SongStorageContext(DbContextOptions<SongStorageContext> options)
            : base(options)
        {
        }

        public DbSet<SpotifySong> Songs { get; set; }
    }
}
