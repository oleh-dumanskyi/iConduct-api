using IConduct.Application.Interfaces;
using IConduct.Application.Managers;
using IConduct.Domain.Interfaces.Repositories;
using IConduct.Infrastructure.Repositories;

namespace IConduct.API.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeManager, EmployeeManager>();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            return services;
        }
    }
}
