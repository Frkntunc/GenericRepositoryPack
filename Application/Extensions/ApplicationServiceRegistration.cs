using ApplicationService.Features.Common;
using ApplicationService.Features.Common.Application.Common.Behaviors;
using ApplicationService.Repositories;
using ApplicationService.Services;
using ApplicationService.SharedKernel;
using ApplicationService.SharedKernel.Auth;
using ApplicationService.SharedKernel.Auth.Common;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Options;
using Shared.Options;
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
                var serviceProvider = services.BuildServiceProvider();
                var redisOptions = serviceProvider.GetRequiredService<IOptions<CacheOptions>>().Value;
                options.Configuration = redisOptions.RedisConfiguration;
                options.InstanceName = redisOptions.RedisInstanceName;
            });

            services.AddScoped<ICacheService, CacheService>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

            services.AddScoped<IDeadLetterService, DeadLetterService>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RetryAndDeadLetterBehavior<,>));

            return services;
        }
    }
}
