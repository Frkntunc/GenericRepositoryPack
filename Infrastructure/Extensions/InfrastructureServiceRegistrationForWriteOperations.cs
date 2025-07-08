using Infrastructure.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ApplicationService.Repositories.Common;
using Infrastructure.Repositories.Common;
using ApplicationService.Repositories;
using Infrastructure.Repositories;
using Users.Infrastructure.Repositories;

namespace Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistrationForWriteOperations
    {
        public static IServiceCollection AddInfrastructureServiceForWrite(this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContext<WriteDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("WriteConnectionString")));

            services.AddTransient(typeof(IWriteRepositoryBase<>), typeof(WriteRepositoryBase<>));
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddSingleton<IEventStore, EventStore>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
