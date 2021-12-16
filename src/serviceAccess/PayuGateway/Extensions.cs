using Microsoft.Extensions.DependencyInjection;
using PayuGateway.ApiClient;
using System;
using Refit;
using System.Net.Http;
using Shared.Options;
using Shared.PaymentMethods;

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
