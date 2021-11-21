using System.Collections.Generic;

namespace Shared.PaymentMethods.Dtos
{
    public record TransactionDto
    {
        public IEnumerable<ProductDto> Products { get; init; }
        public string BuyerPhone { get; set; }
    }
}
