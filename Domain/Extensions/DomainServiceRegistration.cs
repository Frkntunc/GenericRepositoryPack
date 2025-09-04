using Domain.Services.Abstract;
using Domain.Services.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Extensions
{
    public static class DomainServiceRegistration
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IRefreshTokenDomainService, RefreshTokenDomainService>();
            services.AddScoped<IUserDomainService, UserDomainService>();

            return services;
        }
    }
}
