using System.Text.Json.Serialization;

namespace PayuGateway.ApiClient.Dto
{
    public record PayuAuthResponseDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; init; }

        [JsonPropertyName("expires_in")]
        public int ExpirationPeriodInSeconds { get; init; }

        [JsonPropertyName("grant_type")]
        public string GrantType { get; init; }
    }
}
