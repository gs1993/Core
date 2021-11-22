using System.Collections.Generic;

namespace WebApi.Dto.Order
{
    public record CreateOrderDto
    {
        public string Email { get; }
        public string PhoneNumber { get; }
        public string PhoneNumberCountryOrderCode { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public IEnumerable<OrderItemDto> OrderItems { get; }
    }
}
