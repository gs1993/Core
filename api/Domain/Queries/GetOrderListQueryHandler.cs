using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Dto.Order;
using WebApi.Entities.Product;
using WebApi.Helpers;

namespace WebApi.Domain.Queries
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
        private readonly DataContext _context;

        public GetOrderListQueryHandler(IReadonlyDataContext readonlyDataContext, DataContext context)
        {
            _readonlyDataContext = readonlyDataContext ?? throw new ArgumentNullException(nameof(readonlyDataContext));
            _context = context;
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
                .Select(x => OrderDto.FromModel(x))
                .ToList();
        }
    }
}
