using CSharpFunctionalExtensions;
using Domain.Orders.Entities;
using Domain.Shared.DatabaseContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.PaymentMethods;
using Shared.PaymentMethods.Dtos;
using Shared.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Orders.Commands
{
    public record SubmitOrderCommand : IRequest<Result<string>>
    {
        public long OrderId { get; init; }
    }

    public class SubmitOrderCommandHandler : IRequestHandler<SubmitOrderCommand, Result<string>>
    {
        private readonly DataContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPaymentGateway _paymentGateway;

        public SubmitOrderCommandHandler(DataContext context, IDateTimeProvider dateTimeProvider, IPaymentGateway paymentGateway)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _paymentGateway = paymentGateway;
        }

        public async Task<Result<string>> Handle(SubmitOrderCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var order = await GetOrder(request.OrderId, cancellationToken);
            if (order == null)
                return Result.Failure<string>("Order not found");

            await SetOrderSubmitted(order, cancellationToken);

            var transactionDto = CreateTransactionDto(order);
            var paymentResult = await _paymentGateway.SubmitOrder(transactionDto, cancellationToken);
            if (paymentResult.IsFailure)
            {
                await SetPaymentError(order, cancellationToken);
                return Result.Failure<string>(paymentResult.Error);
            }

            await SetPaymentInProgress(order, cancellationToken);

            return Result.Success(paymentResult.Value);
        }


        private Task<Order> GetOrder(long orderId, CancellationToken cancellationToken)
        {
            return _context.Orders
                .Include(x => x.OrderItems)
                .SingleOrDefaultAsync(x => x.Id == orderId && x.OrderState.Value == OrderStateEnum.Created, cancellationToken: cancellationToken);
        }

        private Task SetPaymentError(Order order, CancellationToken cancellationToken)
        {
            order.SetPaymentError(_dateTimeProvider.Now);
            return _context.SaveChangesAsync(cancellationToken);
        }

        private Task SetPaymentInProgress(Order order, CancellationToken cancellationToken)
        {
            order.SetPaymentInProgress(_dateTimeProvider.Now);
            return _context.SaveChangesAsync(cancellationToken);
        }

        private Task SetOrderSubmitted(Order order, CancellationToken cancellationToken)
        {
            order.SetOrderSubmitted(_dateTimeProvider.Now);
            return _context.SaveChangesAsync(cancellationToken);
        }

        private static TransactionDto CreateTransactionDto(Order order)
        {
            return new TransactionDto
            {
                Email = order.Email,
                Phone = order.PhoneNumber.ToString(),
                FirstName = order.Name.FirstName,
                LastName = order.Name.LastName,
                TotalAmount = order.FullPrice.Value,
                CurrencyCode = order.FullPrice.Currency,
                Language = "pl",
                Products = order.OrderItems.Select(x => new ProductDto
                {
                    Name = x.Product.Name,
                    Quantity = x.Quantity,
                    UnitPrice = x.Product.Price.Value
                })
            };
        }
    }
}
