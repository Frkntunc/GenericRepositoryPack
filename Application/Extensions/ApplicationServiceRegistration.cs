using ApplicationService.SharedKernel.Auth.Common;
using ApplicationService.SharedKernel;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ApplicationService.Services;
using ApplicationService.SharedKernel.Auth;
using Microsoft.Extensions.Configuration;

namespace ApplicationService.Extensions
{
    public static class ApplicationServiceRegistration
    {
        //Application'da kullanmak istediğimiz servisleri implemente ediyoruz
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddScoped<IUserContext, UserContext>();
            services.AddSingleton<JwtTokenService>();
            services.AddScoped<RefreshTokenService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();

            return services;
        }
    }
}
