using ApplicationService.Repositories;
using ApplicationService.Repositories.Common;
using Infrastructure.Consumers;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Infrastructure.Repositories;

namespace Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddTransient(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<CacheInvalidatedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitConfig = configuration.GetSection("RabbitMq");

                    cfg.Host(rabbitConfig["Host"], h =>
                    {
                        h.Username(rabbitConfig["Username"]);
                        h.Password(rabbitConfig["Password"]);
                    });

                    cfg.ReceiveEndpoint("cache-invalidation-queue", e =>
                    {
                        e.ConfigureConsumer<CacheInvalidatedConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
