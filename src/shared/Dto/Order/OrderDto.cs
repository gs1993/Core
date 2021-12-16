using System.Collections.Generic;

namespace Shared.Dto.Order
{
    public record OrderDto
    {
        public long Id { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public List<OrderListItemDto> OrderItems { get; set; }
    }
}
