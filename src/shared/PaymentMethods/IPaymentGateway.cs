using CSharpFunctionalExtensions;
using Shared.PaymentMethods.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.PaymentMethods
{
    public interface IPaymentGateway
    {
        public Task<Result<string>> SubmitOrder(TransactionDto dto, CancellationToken cancellationToken);
    }
}
