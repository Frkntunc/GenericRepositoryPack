using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence.Contracts;
using Microsoft.Extensions.Options;
using Shared.Options;
using Persistence.Interceptors;

namespace Infrastructure.Extensions
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<AuditInterceptor>();

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<AuditInterceptor>();

                var connOptions = sp.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value;
                options.UseSqlServer(connOptions.SqlServerConnectionString).AddInterceptors(interceptor);
            });

            services.AddScoped<MigrationTracker>(sp =>
            {
                var context = sp.GetRequiredService<AppDbContext>();
                var migrator = context.GetService<IMigrator>();
                var logger = sp.GetRequiredService<ILogger<MigrationTracker>>();

                return new MigrationTracker(context, migrator, logger);
            });

            return services;
        }
    }

}
