using Newtonsoft.Json;

namespace TechTestBackend.Models
{
    public class ResponseToken
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }
    }
}
