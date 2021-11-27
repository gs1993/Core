namespace WebApi.Dto.Order
{
    public record OrderItemDto
    {
        public long ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
