using System.Collections.Generic;

namespace Shared.PaymentMethods.Dtos
{
    public record TransactionDto
    {
        public IEnumerable<ProductDto> Products { get; init; }
        public string Email { get; init; }
        public string Phone { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Language { get; init; }
        public decimal TotalAmount { get; init; }
        public string CurrencyCode { get; init; }
    }

    public record SaleResultDto
    {

    }
}
