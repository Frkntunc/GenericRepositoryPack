using ApplicationService.Repositories;
using ApplicationService.Repositories.Common;
using Infrastructure.Consumers;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistence.Contracts;
using Persistence.Interceptors;
using Shared.Options;
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
                    var serviceProvider = services.BuildServiceProvider();
                    var queueOptions = serviceProvider.GetRequiredService<IOptions<QueueOptions>>().Value;

                    cfg.Host(queueOptions.RabbitMqHost, h =>
                    {
                        h.Username(queueOptions.RabbitMqUsername);
                        h.Password(queueOptions.RabbitMqPassword);
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
