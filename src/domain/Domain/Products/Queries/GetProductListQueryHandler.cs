using Domain.Products.Entities;
using Domain.Shared.DatabaseContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Dto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Products.Queries
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
        private readonly IReadonlyDataContext _readonlyDataContext;

        public GetProductListQueryHandler(IReadonlyDataContext readonlyDataContext)
        {
            _readonlyDataContext = readonlyDataContext ?? throw new ArgumentNullException(nameof(readonlyDataContext));
        }

        public async Task<IReadOnlyList<ProductDto>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Product> productsQuery = _readonlyDataContext.GetQuery<Product>();

            if (!string.IsNullOrWhiteSpace(request.Name))
                productsQuery = productsQuery.Where(x => x.Name == request.Name);

            if (request.MinQuantity.HasValue)
                productsQuery = productsQuery.Where(x => x.Quantity >= request.MinQuantity.Value);

            if (request.MaxQuantity.HasValue)
                productsQuery = productsQuery.Where(x => x.Quantity <= request.MaxQuantity.Value);

            if (request.MinPrice.HasValue)
                productsQuery = productsQuery.Where(x => x.Price.Value >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                productsQuery = productsQuery.Where(x => x.Price.Value <= request.MinPrice.Value);

            if (!string.IsNullOrEmpty(request.Currency))
                productsQuery = productsQuery.Where(x => x.Price.Currency == request.Currency);

            var products = await productsQuery.ToListAsync(cancellationToken);
            return products
                .Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreateDate = x.CreateDate,
                    Description = x.Description,
                    Price = x.Price.Value,
                    Currency = x.Price.Currency,
                    Quantity = x.Quantity
                })
                .ToList();
        }
    }
}
