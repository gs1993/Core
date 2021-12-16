using System.Text.Json.Serialization;

namespace PayuGateway.ApiClient.Dto
{
    public record CreateOrderResponseDto
    {
        [JsonPropertyName("status")]
        public StatusDto Status { get; init; }

        [JsonPropertyName("redirectUri")]
        public string RedirectUri { get; init; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; init; }

        [JsonPropertyName("extOrderId")]
        public string ExtOrderId { get; init; }

        public record StatusDto
        {
            [JsonPropertyName("statusCode")]
            public string StatusCode { get; init; }
        }
    }
}
