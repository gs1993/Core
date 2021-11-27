using System.Collections.Generic;
using System.Linq;

namespace WebApi.Dto.Order
{
    public record OrderDto
    {
        public long Id { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public List<OrderListItemDto> OrderItems { get; set; }

        public static OrderDto FromModel(Entities.Product.Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                Email = order.Email.ToString(),
                PhoneNumber = order.PhoneNumber.ToString(),
                FirstName = order.Name.FirstName,
                LastName = order.Name.LastName,
                OrderItems = order.OrderItems.Select(x => new OrderListItemDto
                {
                    Id = x.Id,
                    ProductName = x.Product.Name,
                    Price = x.Product.Price.ToString(),
                    Quantity = x.Quantity,
                    Description = x.Product.Description,
                }).ToList()
            };
        }
    }
}
