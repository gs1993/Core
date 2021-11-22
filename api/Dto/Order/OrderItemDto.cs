namespace WebApi.Dto.Order
{
    public record OrderItemDto
    {
        public long ProductId { get; }
        public int Quantity { get; }
    }
}
