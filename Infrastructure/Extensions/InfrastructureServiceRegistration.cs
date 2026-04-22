using ApplicationService.Repositories.Common;
using Infrastructure.Consumers;
using Infrastructure.Repositories.Common;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Options;

namespace Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddTransient(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddRepositoriesByConvention();



            services.AddMassTransit(x =>
            {
                x.AddConsumer<CacheInvalidatedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var queueOptions = context.GetRequiredService<IOptions<QueueOptions>>().Value;

                    cfg.Host(queueOptions.RabbitMqHost, h =>
                    {
                        h.Username(queueOptions.RabbitMqUsername);
                        h.Password(queueOptions.RabbitMqPassword);
                    });

                    cfg.ReceiveEndpoint("cache-invalidation-queue", e =>
                    {
                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(1)));
                        e.UseInMemoryOutbox(context);
                        e.ConfigureConsumer<CacheInvalidatedConsumer>(context);
                    });
                });
            });

            return services;
        }

        private static void AddRepositoriesByConvention(this IServiceCollection services)
        {
            var repositoryBaseType = typeof(IRepositoryBase<>);
            var infrastructureAssembly = typeof(RepositoryBase<>).Assembly;
            var applicationAssembly = repositoryBaseType.Assembly;

            var repositoryInterfaces = applicationAssembly.GetExportedTypes()
                .Where(t => t.IsInterface
                    && t != repositoryBaseType
                    && t.GetInterfaces().Any(i =>
                        i.IsGenericType && i.GetGenericTypeDefinition() == repositoryBaseType));

            var concreteTypes = infrastructureAssembly.GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract);

            foreach (var interfaceType in repositoryInterfaces)
            {
                var implementationType = concreteTypes
                    .FirstOrDefault(t => interfaceType.IsAssignableFrom(t));

                if (implementationType != null)
                {
                    services.AddScoped(interfaceType, implementationType);
                }
            }
        }
    }
}
