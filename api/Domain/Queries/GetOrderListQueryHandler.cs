using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Dto.Order;
using WebApi.Helpers;

namespace WebApi.Domain.Queries
{
    public record GetOrderListQuery : IRequest<IReadOnlyList<OrderDto>>
    {
        public string Email { get; init; }
        public decimal? MinOrderValue { get; init; }
        public decimal? MaxOrderValue { get; init; }
        public long? ProductId { get; init; }
        public int? MinOrderItems { get; init; }
        public int? MaxOrderItems { get; init; }
    }

    public class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, IReadOnlyList<OrderDto>>
    {
        private readonly QueriesConnectionString _connectionString;

        public GetOrderListQueryHandler(QueriesConnectionString connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IReadOnlyList<OrderDto>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
        {
            var builder = new SqlBuilder();
            var selector = builder.AddTemplate(@"
    SELECT o.Id, o.Email, o.PhoneNumber, o.CountryCallingCode as PhoneNumberCountryOrderCode, o.FirstName, o.LastName,
        p.[Name], oi.Quantity, p.Price, p.Currency
    FROM Orders o
    JOIN OrderItems oi ON o.Id = oi.OrderId
    JOIN Products p ON oi.ProductId = p.Id /**where**/");
            builder.Where("o.IsDeleted = 0");

            if (!string.IsNullOrWhiteSpace(request.Email))
                builder.Where("Email = @Email", new { request.Email });

            if (request.MinOrderValue.HasValue)
                builder.Where("OrderValue >= @MinOrderValue", new { request.MinOrderValue });

            if (request.MaxOrderValue.HasValue)
                builder.Where("OrderValue <= @MaxOrderValue", new { request.MaxOrderValue });

            //if (request.ProductId.HasValue)
            //    builder.Where("??? >= @ProductId", new { request.ProductId });

            //if (request.MinOrderItems.HasValue)
            //    builder.Where("OrderItems >= @MinOrderItems", new { request.MinOrderItems });

            //if (request.MaxOrderItems.HasValue)
            //    builder.Where("OrderItems <= @MaxOrderItems", new { request.MaxOrderItems });

            using var connection = new SqlConnection(_connectionString.Value);
            var orderDictionary = new Dictionary<long, OrderDto>();
            var result = connection.Query<OrderDto, OrderListItemDto, OrderDto>(selector.RawSql,
                (order, orderDetail) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out OrderDto orderEntry))
                    {
                        orderEntry = order;
                        orderEntry.OrderItems = new List<OrderListItemDto>();
                        orderDictionary.Add(orderEntry.Id, orderEntry);
                    }

                    orderEntry.OrderItems.Add(orderDetail);
                    return orderEntry;
                }, splitOn: "o.Id");
            return result.ToList();
        }
    }
}
