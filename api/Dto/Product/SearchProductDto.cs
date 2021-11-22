namespace WebApi.Dto.Product
{
    public record SearchProductDto
    {
        public string Name { get; init; }
        public int? MinQuantity { get; init; }
        public int? MaxQuantity { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }
        public string Currency { get; init; }
    }
}
