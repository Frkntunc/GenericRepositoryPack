using Microsoft.Extensions.DependencyInjection;

namespace Domain.Extensions
{
    public static class DomainServiceRegistration
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
