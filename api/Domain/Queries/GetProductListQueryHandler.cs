using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Dto.Product;
using WebApi.Helpers;

namespace WebApi.Domain.Queries
{
    public record GetProductListQuery : IRequest<IReadOnlyList<ProductDto>>
    {
        public string Name { get; init; }
        public int? MinQuantity { get; init; }
        public int? MaxQuantity { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }
        public string Currency { get; init; }
    }

    public class GetProductListQueryHandler : IRequestHandler<GetProductListQuery, IReadOnlyList<ProductDto>>
    {
        private readonly QueriesConnectionString _connectionString;

        public GetProductListQueryHandler(QueriesConnectionString connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IReadOnlyList<ProductDto>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
        {
            var builder = new SqlBuilder();
            var selector = builder.AddTemplate("SELECT Id, Name, Description, Quantity, Price, Currency, CreateDate FROM Products /**where**/");
            builder.Where("IsDeleted = 0");

            if (!string.IsNullOrWhiteSpace(request.Name))
                builder.Where("Name = @Name", new { request.Name });

            if (request.MinQuantity.HasValue)
                builder.Where("Quantity >= @MinQuantity", new { request.MinQuantity });

            if (request.MaxQuantity.HasValue)
                builder.Where("Quantity <= @MaxQuantity", new { request.MaxQuantity });

            if (request.MinPrice.HasValue)
                builder.Where("Price >= @MinPrice", new { request.MinPrice });

            if (request.MaxPrice.HasValue)
                builder.Where("Price <= @MaxPrice", new { request.MaxPrice });

            if (!string.IsNullOrEmpty(request.Currency))
                builder.Where("Currency = @Currency", new { request.Currency });

            using var connection = new SqlConnection(_connectionString.Value);
            var result = await connection.QueryAsync<ProductDto>(selector.RawSql, cancellationToken);
            return result.ToList();
        }
    }
}
