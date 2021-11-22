namespace WebApi.Dto.Order
{
    public record SearchOrderDto
    {
        public string Email { get; }
        public decimal? MinOrderValue { get; }
        public decimal? MaxOrderValue { get; }
        public long? ProductId { get; }
        public int? MinOrderItems { get; }
        public int? MaxOrderItems { get; }
    }
}
