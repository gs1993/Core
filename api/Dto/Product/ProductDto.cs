using System;

namespace WebApi.Dto.Product
{
    public record ProductDto
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public int Quantity { get; init; }
        public decimal Price { get; init; }
        public string Currency { get; init; }
        public string Description { get; init; }
        public DateTime CreateDate { get; init; }
    }
}
