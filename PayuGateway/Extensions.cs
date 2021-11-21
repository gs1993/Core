using Microsoft.Extensions.DependencyInjection;
using PayuGateway.ApiClient;
using Shared.Options;
using System;
using Refit;
using Shared.PaymentMethods;

namespace PayuGateway
{
    public static class Extensions
    {
        public static void AddPayuGateway(this IServiceCollection services, PayuGatewaySettings settings)
        {
            services.AddSingleton(settings);

            services
                .AddRefitClient<IPayuApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(settings.BaseAddress));

            services.AddTransient<IPaymentGateway, PayuGatewayPaymentService>();
        }
    }
}
