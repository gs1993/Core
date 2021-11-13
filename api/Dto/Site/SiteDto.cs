namespace WebApi.Dto.Site
{
    public record SiteDto
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string AddressFirstLine { get; init; }
        public string AddressSecondLine { get; init; }
    }
}
