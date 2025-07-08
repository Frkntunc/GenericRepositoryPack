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

namespace Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistrationForReadOperations
    {
        public static IServiceCollection AddInfrastructureServiceForRead(this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContext<ReadDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("MongoDbSettings")));

            services.AddTransient(typeof(IReadRepositoryBase<>), typeof(ReadRepositoryBase<>));
            //services.AddTransient<IApartmentRepository, ApartmentRepository>();

            return services;
        }
    }
}
