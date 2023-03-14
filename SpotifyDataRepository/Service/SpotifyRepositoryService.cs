using System.Net.Http;
using Domain;

namespace SpotifyDataRepository.Service
{
    public class SpotifyRepositoryService : ISpotifyRepositoryService
    {
        private SongStorageContext _songStorageContext;

        public SpotifyRepositoryService(SongStorageContext songStorageContext)
        {
            _songStorageContext = songStorageContext ?? throw new ArgumentNullException(nameof(songStorageContext));
        }

        public async Task AddSong(SpotifySong song)
        {
            try
            {
                // check if item already exists in database
                var existingItem = _songStorageContext.Songs.SingleOrDefault(x => x.Name == song.Name);

                if (existingItem == null)
                {
                    // item does not exist, so add it to the database
                    _songStorageContext.Songs.Add(song);
                    await _songStorageContext.SaveChangesAsync();
                }
                else
                {
                    // item already exists, do not add it to the database
                    Console.WriteLine("Item already exists in database!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the item: {ex.Message}");
            }
        }

        public async Task DeleteSong(string id)
        {
            try
            {
                // check if item exists in database
                var existingItem = _songStorageContext.Songs.SingleOrDefault(x => x.Id == id);

                if (existingItem != null)
                {
                    // item exists, so delete it from the database
                    _songStorageContext.Songs.Remove(existingItem);
                    await _songStorageContext.SaveChangesAsync();
                }
                else
                {
                    // item does not exist, do not delete it from the database
                    Console.WriteLine("Item does not exist in database!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting the item: {ex.Message}");
            }
        }

        private bool SongExists(string id)
        {
            return  _songStorageContext.Songs.SingleOrDefault(x => x.Id == id) != null;
        }
    }
}
