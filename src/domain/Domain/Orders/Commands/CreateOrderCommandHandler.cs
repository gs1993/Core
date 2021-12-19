using CSharpFunctionalExtensions;
using Domain.Orders.Entities;
using Domain.Products.Entities;
using Domain.Shared.DatabaseContext;
using Domain.Shared.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Orders.Commands
{
    public record CreateOrderCommand : IRequest<Result<long>>
    {
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string PhoneNumberCountryOrderCode { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public IEnumerable<OrderItemCommandDto> OrderItems { get; init; }

        public record OrderItemCommandDto
        {
            public long ProductId { get; init; }
            public int Quantity { get; init; }
        }
    }

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<long>>
    {
        private readonly DataContext _context;

        public CreateOrderCommandHandler(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<long>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var emailResult = Email.Create(request.Email);
            if (emailResult.IsFailure)
                return Result.Failure<long>(emailResult.Error);

            var nameResult = Name.Create(string.Empty, request.FirstName, request.LastName);
            if (nameResult.IsFailure)
                return Result.Failure<long>(nameResult.Error);

            var phoneNumberResult = PhoneNumber.Create(request.PhoneNumber, request.PhoneNumberCountryOrderCode);
            if (phoneNumberResult.IsFailure)
                return Result.Failure<long>(phoneNumberResult.Error);

            var orderResult = Order.Create(emailResult.Value, phoneNumberResult.Value, nameResult.Value);
            if (orderResult.IsFailure)
                return Result.Failure<long>(orderResult.Error);

            var order = orderResult.Value;
            var products = await GetOrderedProducts(request, cancellationToken);
            foreach (var orderItem in request.OrderItems)
            {
                var product = products.FirstOrDefault(x => x.Id == orderItem.ProductId);
                var addOrderItemResult = order.AddOrderItem(product, orderItem.Quantity);
                if (addOrderItemResult.IsFailure)
                    return Result.Failure<long>(addOrderItemResult.Error);
            }

            _context.Orders.Attach(order);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(order.Id);
        }


        private Task<List<Product>> GetOrderedProducts(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var productIds = request.OrderItems
                .Select(x => x.ProductId);

            return _context.Products
                .Where(x => productIds.Contains(x.Id))
                .ToListAsync(cancellationToken: cancellationToken);
        }
    }
}
