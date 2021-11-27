using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Entities.Product;
using WebApi.Entities.Shared;
using WebApi.Helpers;

namespace WebApi.Domain.Commands
{
    public record CreateOrderCommand : IRequest<Result>
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

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result>
    {
        private readonly DataContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CreateOrderCommandHandler(DataContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public async Task<Result> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var emailResult = Email.Create(request.Email);
            if (emailResult.IsFailure)
                return emailResult;

            var nameResult = Name.Create(string.Empty, request.FirstName, request.LastName);
            if (nameResult.IsFailure)
                return nameResult;

            var phoneNumberResult = PhoneNumber.Create(request.PhoneNumber, request.PhoneNumberCountryOrderCode);
            if (phoneNumberResult.IsFailure)
                return phoneNumberResult;

            var orderCreateDate = _dateTimeProvider.Now;
            var orderResult = Order.Create(emailResult.Value, phoneNumberResult.Value, nameResult.Value, orderCreateDate);
            if (orderResult.IsFailure)
                return orderResult;

            var order = orderResult.Value;
            var products = await GetOrderedProducts(request, cancellationToken);
            foreach (var orderItem in request.OrderItems)
            {
                var product = products.FirstOrDefault(x => x.Id == orderItem.ProductId);
                var addOrderItemResult = order.AddOrderItem(product, orderItem.Quantity, orderCreateDate);
                if (addOrderItemResult.IsFailure)
                    return addOrderItemResult;
            }

            _context.Orders.Attach(order);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        private async Task<List<Product>> GetOrderedProducts(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var productIds = request.OrderItems
                .Select(x => x.ProductId);

            return await _context.Products
                .Where(x => productIds.Contains(x.Id))
                .ToListAsync(cancellationToken: cancellationToken);
        }
    }
}
