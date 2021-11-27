namespace WebApi.Dto.Order
{
    public record CreateOrderItemDto
    {
        public long ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
