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
    public record InitiatePaymentCommand : IRequest<Result<string>>
    {
        public long OrderId { get; init; }
    }

    public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, Result<string>>
    {
        private readonly DataContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPaymentGateway _paymentGateway;

        public InitiatePaymentCommandHandler(DataContext context, IDateTimeProvider dateTimeProvider, IPaymentGateway paymentGateway)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _paymentGateway = paymentGateway;
        }

        public async Task<Result<string>> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var order = await _context.Orders
                .Include(x => x.OrderItems)
                .SingleOrDefaultAsync(x => x.Id == request.OrderId && x.OrderState.Value == OrderStateEnum.Created, cancellationToken: cancellationToken);
            if (order == null)
                return Result.Failure<string>("Order not found");

            order.SetPaymentStarted(_dateTimeProvider.Now);
            await _context.SaveChangesAsync(cancellationToken);

            var transactionDto = CreateTransactionDto(order);
            var paymentResult = await _paymentGateway.Sale(transactionDto, cancellationToken);
            if (paymentResult.IsFailure)
            {
                order.SetPaymentError(_dateTimeProvider.Now);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Failure<string>(paymentResult.Error);
            }

            order.SetPaymentInProgress(_dateTimeProvider.Now);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(paymentResult.Value);
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
