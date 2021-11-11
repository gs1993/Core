namespace WebApi.Helpers
{
    public record AppSettings
    {
        public string Secret { get; init; }
        public int RefreshTokenTtlInDays { get; init; }
        public EmailSettings EmailSettings { get; set; }
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