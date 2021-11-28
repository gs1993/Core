using System.Collections.Generic;

namespace PayuGateway.ApiClient.Dto
{
    public record CreateOrderDto
    {
        public string CustomerIp { get; init; }
        public string CustomerId { get; init; }
        public string MerchantPosId { get; init; }
        public string Description { get; init; }
        public int TotalAmount { get; init; }
        public string CurrencyCode { get; init; }
        public BuyerDto Buyer { get; init; }
        public IEnumerable<ProductsDto> Products { get; init; }

        public record BuyerDto
        {
            public string Email { get; init; }
            public string Phone { get; init; }
            public string FirstName { get; init; }
            public string LastName { get; init; }
            public string Language { get; init; }
        }
        public record ProductsDto
        {
            public string Name { get; init; }
            public int UnitPrice { get; init; }
            public int Quantity { get; init; }
        }
    }
}
