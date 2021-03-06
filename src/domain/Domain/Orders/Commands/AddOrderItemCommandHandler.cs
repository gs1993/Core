using CSharpFunctionalExtensions;
using Domain.Shared.DatabaseContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Orders.Commands
{
    public record AddOrderItemCommand : IRequest<Result>
    {
        public long OrderId { get; set; }
        public long ProductId { get; init; }
        public int Quantity { get; init; }
    }

    public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, Result>
    {
        private readonly DataContext _context;

        public AddOrderItemCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var order = await _context.Orders
                .Include(x => x.OrderItems)
                .SingleOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken: cancellationToken);
            if (order == null)
                return Result.Failure("Order not found");

            var productToAdd = await _context.Products.SingleOrDefaultAsync(x => x.Id == request.ProductId, cancellationToken: cancellationToken);
            if (productToAdd == null)
                return Result.Failure("Product not found");

            var addOrderItemResult = order.AddOrderItem(productToAdd, request.Quantity);
            if (addOrderItemResult.IsFailure)
                return addOrderItemResult;

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
