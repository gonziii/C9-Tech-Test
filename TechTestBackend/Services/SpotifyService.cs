using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Domain;
using TechTestBackend.Client;
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

    private async Task<string?> GetSpotifyToken()
    {
        var byteArray = Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}");
        var request = new HttpRequestMessage(HttpMethod.Post, "api/token")
        {
            Content = new StringContent("grant_type=client_credentials"),
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseToken>(content);
            return result?.AccessToken;
        }
    }

    public async Task<List<SpotifySong>?> GetTracks(string name)
    {
        var token = await GetSpotifyToken();
        if (token == null)
        {
            throw new Exception("Could not get token");
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var request = new HttpRequestMessage(HttpMethod.Get, $"v1/search?q={name}&type=track");
        using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var songs = JsonConvert.DeserializeObject<List<SpotifySong>>(content);
            return songs;
        }
    }

    public async Task<SpotifySong?> GetTrack(string id)
    {
        var token = await GetSpotifyToken();
        if (token == null)
        {
            throw new Exception("Could not get token");
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var request = new HttpRequestMessage(HttpMethod.Get, $"v1/tracks/{id}/");
        using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var song = JsonConvert.DeserializeObject<SpotifySong>(content);
            return song;
        }
    }
}

