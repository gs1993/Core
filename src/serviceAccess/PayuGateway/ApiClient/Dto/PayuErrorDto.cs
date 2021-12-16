using System.Text.Json.Serialization;

namespace PayuGateway.ApiClient.Dto
{
    public record PayuErrorDto
    {
        [JsonPropertyName("status")]
        public PayuErrorStatus Status { get; set; }

        public record PayuErrorStatus
        {
            [JsonPropertyName("statusCode")]
            public string StatusCode { get; set; }

            [JsonPropertyName("severity")]
            public string Severity { get; set; }

            [JsonPropertyName("code")]
            public string Code { get; set; }

            [JsonPropertyName("codeLiteral")]
            public string CodeLiteral { get; set; }

            [JsonPropertyName("statusDesc")]
            public string StatusDesc { get; set; }
        }
    }
}
