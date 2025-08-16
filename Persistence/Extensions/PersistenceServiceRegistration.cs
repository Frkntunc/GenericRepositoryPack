using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("ConnectionString")));

            services.AddScoped<MigrationTracker>(sp =>
            {
                var context = sp.GetRequiredService<AppDbContext>();
                var migrator = context.GetService<IMigrator>();
                return new MigrationTracker(context, migrator);
            });

            return services;
        }
    }
}
