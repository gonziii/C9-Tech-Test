namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ISpotifyRepository Spotify { get; }
        int Complete();
    }
}
