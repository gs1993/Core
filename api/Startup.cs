using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayuGateway;
using Shared.Options;
using System;
using System.Text.Json.Serialization;
using WebApi.Helpers;
using WebApi.Middleware;
using WebApi.Services;

namespace WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            AddDbContext(services);

            services.AddCors();
            services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.IgnoreNullValues = true;
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddSwaggerGen();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();

            ConfigurePaymentGateway(services);

            services.AddMediatR(typeof(Startup).Assembly);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "Core API"));

            app.UseRouting();

            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(x => x.MapControllers());
        }


        private void AddDbContext(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("WebApiDatabase");
            services.AddDbContext<DataContext>(options => options
                .UseSqlServer(connectionString)
                .UseLazyLoadingProxies()
            );
            services.AddSingleton(new QueriesConnectionString { Value = connectionString });
        }

        private void ConfigurePaymentGateway(IServiceCollection services)
        {
            services.AddPayuGateway(Configuration.GetOptions<PayuGatewaySettings>());
            //TODO: Add configurable gateway options
        }
    }
}
