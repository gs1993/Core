using CSharpFunctionalExtensions;
using PayuGateway.ApiClient;
using Shared.Options;
using Shared.PaymentMethods;
using Shared.PaymentMethods.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PayuGateway
{
    public class PayuGatewayPaymentService : IPaymentGateway
    {
        private readonly PayuGatewaySettings _settings;
        private readonly IPayuApi _api;

        public PayuGatewayPaymentService(PayuGatewaySettings settings, IPayuApi api)
        {
            _settings = settings;
            _api = api;
        }

        public async Task<Result> Sale(TransactionDto dto, CancellationToken cancellationToken)
        {
            var authorizationeResult = await Authorize();
            if (authorizationeResult.IsFailure)
                return authorizationeResult;

            //TODO: Add sale result

            return Result.Success();
        }

        public Task<SetupNewPaymentDto> SetupNewPayment(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<Result<PayuAuthResponseDto>> Authorize()
        {
            var authorizeData = new Dictionary<string, object>()
            {
                { "grant_type", "client_credentials" },
                { "client_id", _settings.ClientId },
                { "client_secret", _settings.ClientSecret }
            };
            var authorizationResponse = await _api.Authorize(authorizeData);
            if (!authorizationResponse.IsSuccessStatusCode)
                return Result.Failure<PayuAuthResponseDto>(authorizationResponse.Error.Message);

            return Result.Success(authorizationResponse.Content);
        }
    }
}
