using Domain.Orders.Entities;
using Domain.Shared.DatabaseContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Dto.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Orders.Queries
{
    public record GetOrderListQuery : IRequest<IReadOnlyList<OrderDto>>
    {
        public string Email { get; init; }
        public string ProductName { get; init; }
        public decimal? MinOrderValue { get; init; }
        public decimal? MaxOrderValue { get; init; }
        public int? MinOrderItems { get; init; }
        public int? MaxOrderItems { get; init; }
    }

    public class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, IReadOnlyList<OrderDto>>
    {
        private readonly IReadonlyDataContext _readonlyDataContext;

        public GetOrderListQueryHandler(IReadonlyDataContext readonlyDataContext)
        {
            _readonlyDataContext = readonlyDataContext ?? throw new ArgumentNullException(nameof(readonlyDataContext));
        }

        public async Task<IReadOnlyList<OrderDto>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Order> ordersQuery = _readonlyDataContext.GetQuery<Order>()
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product);

            if (!string.IsNullOrWhiteSpace(request.Email))
                ordersQuery = ordersQuery.Where(x => x.Email == request.Email);

            if (!string.IsNullOrWhiteSpace(request.ProductName))
                ordersQuery = ordersQuery.Where(x => x.OrderItems.Any(y => y.Product.Name == request.ProductName));

            if (request.MinOrderValue.HasValue)
                ordersQuery.Where(x => x.FullPrice.Value >= request.MinOrderValue);

            if (request.MaxOrderValue.HasValue)
                ordersQuery.Where(x => x.FullPrice.Value >= request.MaxOrderValue);

            if (request.MinOrderItems.HasValue)
                ordersQuery = ordersQuery.Where(x => x.OrderItems.Count >= request.MinOrderItems);

            if (request.MaxOrderItems.HasValue)
                ordersQuery = ordersQuery.Where(x => x.OrderItems.Count <= request.MinOrderItems);

            var orders = await ordersQuery.ToListAsync(cancellationToken);
            return orders
                .Select(x => new OrderDto
                {
                    Id = x.Id,
                    Email = x.Email.ToString(),
                    PhoneNumber = x.PhoneNumber.ToString(),
                    FirstName = x.Name.FirstName,
                    LastName = x.Name.LastName,
                    OrderItems = x.OrderItems.Select(x => new OrderListItemDto
                    {
                        Id = x.Id,
                        ProductName = x.Product.Name,
                        Price = x.Product.Price.ToString(),
                        Quantity = x.Quantity,
                        Description = x.Product.Description,
                    }).ToList()
                }).ToList();
        }
    }
}
