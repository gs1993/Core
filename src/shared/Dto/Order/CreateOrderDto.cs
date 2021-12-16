using System.Collections.Generic;

namespace Shared.Dto.Order
{
    public record CreateOrderDto
    {
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string PhoneNumberCountryOrderCode { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public IEnumerable<OrderItemDto> OrderItems { get; init; }
    }
}
