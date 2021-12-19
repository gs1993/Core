using CSharpFunctionalExtensions;
using PayuGateway.ApiClient;
using PayuGateway.ApiClient.Dto;
using Refit;
using Shared.Options;
using Shared.PaymentMethods;
using Shared.PaymentMethods.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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

        public async Task<Result<string>> SubmitOrder(TransactionDto dto, CancellationToken cancellationToken)
        {
            var authorizationeResult = await Authorize(cancellationToken);
            if (authorizationeResult.IsFailure)
                return Result.Failure<string>(authorizationeResult.Error);

            var response = await SendOrder(dto, authorizationeResult.Value.AccessToken, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Found)
                return DeserializeSuccessRedirect(response);
            if (!response.IsSuccessStatusCode)
                return Result.Failure<string>(GetErrorMessage(response.Error));

            return Result.Success(response.Content.RedirectUri);
        }


        private Task<ApiResponse<CreateOrderResponseDto>> SendOrder(TransactionDto dto, string bearerToken, CancellationToken cancellationToken)
        {
            return _api.CreateOrder(new CreateOrderDto
            {
                CustomerIp = "127.0.0.1",
                CustomerId = _settings.ClientId,
                MerchantPosId = _settings.PosId,
                Description = "Sale Order",
                TotalAmount = (int)(dto.TotalAmount * 100),
                CurrencyCode = dto.CurrencyCode,
                Buyer = new CreateOrderDto.BuyerDto
                {
                    Email = dto.Email,
                    Phone = dto.Phone,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Language = dto.Language
                },
                Products = dto.Products.Select(x => new CreateOrderDto.ProductsDto
                {
                    Name = x.Name,
                    Quantity = x.Quantity,
                    UnitPrice = (int)(x.UnitPrice * 100)
                })
            }, bearerToken, cancellationToken);
        }

        private async Task<Result<PayuAuthResponseDto>> Authorize(CancellationToken cancellationToken)
        {
            var authorizeData = new Dictionary<string, object>()
            {
                { "grant_type", "client_credentials" },
                { "client_id", _settings.ClientId },
                { "client_secret", _settings.ClientSecret }
            };
            var authorizationResponse = await _api.Authorize(authorizeData, cancellationToken);
            if (!authorizationResponse.IsSuccessStatusCode)
                return Result.Failure<PayuAuthResponseDto>(authorizationResponse.Error.Message);

            return Result.Success(authorizationResponse.Content);
        }

        private static Result<string> DeserializeSuccessRedirect(ApiResponse<CreateOrderResponseDto> response)
        {
            var redirectResponse = JsonSerializer.Deserialize<CreateOrderResponseDto>(response.Error.Content);
            if (redirectResponse?.Status?.StatusCode != "SUCCESS")
                throw new InvalidCastException("Cannot process payment gateway response");

            return Result.Success(redirectResponse.RedirectUri);
        }

        private static string GetErrorMessage(ApiException exception)
        {
            if (exception.HasContent)
            {
                var errorModel = JsonSerializer.Deserialize<PayuErrorDto>(exception.Content);
                if (!string.IsNullOrEmpty(errorModel?.Status?.StatusCode))
                    return $"Status: {errorModel.Status.StatusCode} | Code literal: {errorModel.Status.CodeLiteral} | Description: {errorModel.Status.StatusDesc}";
            }

            return exception.Message;
        }
    }
}
