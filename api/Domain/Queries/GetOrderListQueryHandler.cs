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
            var selector = builder.AddTemplate("SELECT  FROM Orders /**where**/");
            builder.Where("IsDeleted = 0");

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
            var result = await connection.QueryAsync<OrderDto>(selector.RawSql, cancellationToken);
            return result.ToList();
        }
    }
}
