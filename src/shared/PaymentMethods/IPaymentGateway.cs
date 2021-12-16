using CSharpFunctionalExtensions;
using Shared.PaymentMethods.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.PaymentMethods
{
    public interface IPaymentGateway
    {
        public Task<SetupNewPaymentDto> SetupNewPayment(CancellationToken cancellationToken);
        public Task<Result<string>> Sale(TransactionDto dto, CancellationToken cancellationToken);
    }
}
