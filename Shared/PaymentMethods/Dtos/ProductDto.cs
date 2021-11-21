namespace Shared.PaymentMethods.Dtos
{
    public record ProductDto
    {
        public string Name { get; init; }
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
    }
}
