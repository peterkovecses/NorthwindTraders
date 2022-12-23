using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Northwind.Application.Interfaces;
using Northwind.Infrastructure.Identity;
using Northwind.Infrastructure.Persistence;
using Northwind.Infrastructure.Persistence.Interceptors;
using Northwind.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<NorthwindContext>(options =>
                options
                    .UseLazyLoadingProxies()
                    .UseSqlServer(configuration.GetConnectionString("NorthwindDatabase")));

            services.AddDbContext<IdentityContext>(options => 
                options.UseSqlServer(configuration.GetConnectionString("IdentityDatabase")));

            services.AddScoped<IdentityContextInitialiser>();
            services.AddScoped<IIdentityService, IdentityService>();

            services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<AuditInterceptor>();

            return services;
        }
    }
}
