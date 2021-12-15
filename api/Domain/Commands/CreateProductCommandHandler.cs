using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Entities.Product;
using WebApi.Helpers;

namespace WebApi.Domain.Commands
{
    public record CreateProductCommand : IRequest<Result>
    {
        public string Name { get; init; }
        public int Quantity { get; init; }
        public decimal Price { get; init; }
        public string Currency { get; init; }
        public string Description { get; init; }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result>
    {
        private readonly DataContext _context;

        public CreateProductCommandHandler(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Result> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var currencyResult = Price.Create(request.Price, request.Currency);
            if (currencyResult.IsFailure)
                return currencyResult;

            var productResult = Product.Create(request.Name, request.Description, request.Quantity, currencyResult.Value);
            if (productResult.IsFailure)
                return productResult;

            var nameAlreadyExists = await _context.Products.AnyAsync(x => x.Name == request.Name, cancellationToken);
            if (nameAlreadyExists)
                return Result.Failure("Product with that name already exists");

            _context.Products.Attach(productResult.Value);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
