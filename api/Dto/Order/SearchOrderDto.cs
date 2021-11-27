namespace WebApi.Dto.Order
{
    public record SearchOrderDto
    {
        public string Email { get; init; }
        public string ProductName { get; init; }
        public decimal? MinOrderValue { get; init; }
        public decimal? MaxOrderValue { get; init; }
        public int? MinOrderItems { get; init; }
        public int? MaxOrderItems { get; init; }
    }
}
