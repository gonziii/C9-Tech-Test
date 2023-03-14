namespace TechTestBackend.Client
{
    public class SpotifyClient
    {
        public HttpClient Client { get; }
        public SpotifyClient(HttpClient client)
        {
            Client = client;
            Client.BaseAddress = new Uri("https://accounts.spotify.com/");
            Client.Timeout = new TimeSpan(0, 0, 30);
            Client.DefaultRequestHeaders.Clear();
        }
    }
}
