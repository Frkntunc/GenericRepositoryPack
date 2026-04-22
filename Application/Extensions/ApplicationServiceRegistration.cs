using ApplicationService.Features.Common;
using ApplicationService.Features.Common.Application.Common.Behaviors;
using ApplicationService.Services.Common;
using ApplicationService.SharedKernel.Auth.Common;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ApplicationService.Extensions
{
    public static class ApplicationServiceRegistration
    {
        //Application'da kullanmak istediğimiz servisleri implemente ediyoruz
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration, params Assembly[] additionalMediatRAssemblies)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            var assemblies = new[] { Assembly.GetExecutingAssembly() }
                .Concat(additionalMediatRAssemblies)
                .ToArray();
            services.AddMediatR(assemblies);

            services.AddServicesByConvention();
            services.AddScoped<IUserContextSetter>(sp => (IUserContextSetter)sp.GetRequiredService<IUserContext>());

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["Cache:RedisConfiguration"];
                options.InstanceName = configuration["Cache:RedisInstanceName"];
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RetryAndDeadLetterBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }

        private static void AddServicesByConvention(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var concreteTypes = assembly.GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract);

            foreach (var implementationType in concreteTypes)
            {
                var serviceInterfaces = implementationType.GetInterfaces()
                    .Where(i => i != typeof(IScopedService)
                        && i != typeof(ISingletonService)
                        && i != typeof(ITransientService));

                if (typeof(IScopedService).IsAssignableFrom(implementationType))
                {
                    RegisterService(services, implementationType, serviceInterfaces, ServiceLifetime.Scoped);
                }
                else if (typeof(ISingletonService).IsAssignableFrom(implementationType))
                {
                    RegisterService(services, implementationType, serviceInterfaces, ServiceLifetime.Singleton);
                }
                else if (typeof(ITransientService).IsAssignableFrom(implementationType))
                {
                    RegisterService(services, implementationType, serviceInterfaces, ServiceLifetime.Transient);
                }
            }
        }

        private static void RegisterService(IServiceCollection services, Type implementationType,
            IEnumerable<Type> serviceInterfaces, ServiceLifetime lifetime)
        {
            var primaryInterface = serviceInterfaces.FirstOrDefault();

            if (primaryInterface != null)
            {
                services.Add(new ServiceDescriptor(primaryInterface, implementationType, lifetime));
            }
            else
            {
                services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));
            }
        }
    }
}
