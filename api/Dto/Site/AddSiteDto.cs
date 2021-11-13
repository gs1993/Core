namespace WebApi.Dto.Site
{
    public record AddSiteDto
    {
        public string Name { get; init; }
        public string AddressFirstLine { get; init; }
        public string AddressSecondLine { get; init; }
    }
}
