using Microsoft.Extensions.DependencyInjection;
using PayuGateway.ApiClient;
using Shared.Options;
using System;
using Refit;
using Shared.PaymentMethods;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace PayuGateway
{
    public static class Extensions
    {
        public static void AddPayuGateway(this IServiceCollection services, PayuGatewaySettings settings)
        {
            services.AddSingleton(settings);

            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var iPayuApi = RestService.For<IPayuApi>(new HttpClient(handler)
            {
                BaseAddress = new Uri(settings.BaseAddress)
            });
            services.AddSingleton(iPayuApi);

            services.AddTransient<IPaymentGateway, PayuGatewayPaymentService>();
        }
    }
}
