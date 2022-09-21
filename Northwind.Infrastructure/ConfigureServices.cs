using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Northwind.Domain.Common.Interfaces;
using Northwind.Infrastructure.Persistence;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<NorthwindContext>(options => options.UseSqlServer(configuration.GetConnectionString("NorthwindDatabase")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
