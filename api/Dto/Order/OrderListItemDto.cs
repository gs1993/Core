namespace WebApi.Dto.Order
{
    public record OrderListItemDto
    {
        public string ProductName { get; init; }
        public string Description { get; init; }
        public int Quantity { get; init; }
        public decimal Price { get; init; }
        public string Currency { get; init; }
    }
}
