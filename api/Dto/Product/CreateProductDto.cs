namespace WebApi.Dto.Product
{
    public record CreateProductDto
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public int Quantity { get; init; }
        public decimal Price { get; init; }
        public string Currency { get; init; }
    }
}
