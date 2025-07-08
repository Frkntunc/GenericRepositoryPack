using ApplicationService.SharedKernel.Auth.Common;
using ApplicationService.SharedKernel;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ApplicationService.Services;
using ApplicationService.SharedKernel.Auth;
using FluentValidation;
using System.Reflection.Metadata;

namespace ApplicationService.Extensions
{
    public static class ApplicationServiceRegistration
    {
        //Application'da kullanmak istediğimiz servisleri implemente ediyoruz
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AssemblyReference).Assembly);
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped<IUserContext, UserContext>();
            services.AddSingleton<JwtTokenService>();
            services.AddScoped<RefreshTokenService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();

            return services;
        }
    }
}
