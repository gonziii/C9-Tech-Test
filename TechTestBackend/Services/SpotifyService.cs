using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Domain;
using TechTestBackend.Models;

namespace TechTestBackend.Services;
public class SpotifyService : ISpotifyService
{
    private readonly JsonSerializerOptions _options;
    private HttpClient _httpClient;
    private readonly string? _clientId;
    private readonly string? _clientSecret;

    public SpotifyService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _clientId = configuration["SpotifyClientId"];
        _clientSecret = configuration["SpotifyClientSecret"];
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    private async Task<ResponseToken?> GetSpotifyToken()
    {
        var byteArray = Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}");
        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
        {
            Content = new StringContent("grant_type=client_credentials"),
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ResponseToken>(content, _options);
            return result;
        }
    }

    public async Task<List<SpotifySong>?> GetTracks(string name)
    {
        var token = await GetSpotifyToken();
        if (token == null)
        {
            throw new Exception("Could not get token");
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/search?q={name}&type=track");
        using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var songs = JsonSerializer.Deserialize<TracksModel>(content, _options);

                return songs?.Tracks.Items.Select(x => new SpotifySong
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
        }
    }

    public async Task<SpotifySong?> GetTrack(string id)
    {
        var token = await GetSpotifyToken();
        if (token == null)
        {
            throw new Exception("Could not get token");
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/tracks/{id}/");
        using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var song = JsonSerializer.Deserialize<TrackId>(content);
            if (song != null)
                return new SpotifySong
                {
                    Id = song.Id,
                    Name = song.Name,
                };
            return null;
        }
    }
}

