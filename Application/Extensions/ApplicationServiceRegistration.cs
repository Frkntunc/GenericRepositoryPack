using ApplicationService.Features.Common;
using ApplicationService.Repositories;
using ApplicationService.Services;
using ApplicationService.SharedKernel;
using ApplicationService.SharedKernel.Auth;
using ApplicationService.SharedKernel.Auth.Common;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Reflection.Metadata;

namespace ApplicationService.Extensions
{
    public static class ApplicationServiceRegistration
    {
        //Application'da kullanmak istediğimiz servisleri implemente ediyoruz
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(AssemblyReference).Assembly);
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped<IUserContext, UserContext>();
            services.AddSingleton<JwtTokenService>();
            services.AddScoped<RefreshTokenService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "AppCache_";
            });

            services.AddScoped<ICacheService, CacheService>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

            return services;
        }
    }
}
