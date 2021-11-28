using PayuGateway.ApiClient.Dto;
using Refit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PayuGateway.ApiClient
{
    public interface IPayuApi
    {
        [Post("/pl/standard/user/oauth/authorize")]
        Task<ApiResponse<PayuAuthResponseDto>> Authorize([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data, CancellationToken cancellationToken);

        [Post("/api/v2_1/orders")]
        Task<ApiResponse<CreateOrderResponseDto>> CreateOrder(CreateOrderDto dto, [Authorize("Bearer")] string token, CancellationToken cancellationToken);
    }
}
