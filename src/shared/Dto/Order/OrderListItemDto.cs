namespace Shared.Dto.Order
{
    public record OrderListItemDto
    {
        public long Id { get; init; }
        public string ProductName { get; init; }
        public string Description { get; init; }
        public int Quantity { get; init; }
        public string Price { get; init; }
    }
}
