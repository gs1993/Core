using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PayuGateway.ApiClient
{
    public interface IPayuApi
    {
        [Post("/pl/standard/user/oauth/authorize")]
        Task<ApiResponse<PayuAuthResponseDto>> Authorize([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);
    }

    public record PayuAuthResponseDto
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; init; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; init; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpirationPeriodInSeconds { get; init; }

        [JsonProperty(PropertyName = "grant_type")]
        public string GrantType { get; init; }
    }
}
