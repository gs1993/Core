namespace Shared.Options
{
    public record AppSettings
    {
        public string Secret { get; init; }
        public int ExpirationTimeInMinutes { get; init; }
        public int RefreshTokenTtlInDays { get; init; }
        public EmailSettings EmailSettings { get; set; }
    }

    public record BraintreeGatewaySettings
    {
        public string Environment { get; init; }
        public string MerchantId { get; init; }
        public string PublicKey { get; init; }
        public string PrivateKey { get; init; }
    }

    public record PayuGatewaySettings
    {
        public string Environment { get; init; }
        public string BaseAddress { get; init; }
        public string PosId { get; init; }
        public string SecondKey { get; init; }
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
    }

    public record EmailSettings
    {
        public string EmailFrom { get; init; }
        public string SmtpHost { get; init; }
        public int SmtpPort { get; init; }
        public string SmtpUser { get; init; }
        public string SmtpPass { get; init; }
    }
}