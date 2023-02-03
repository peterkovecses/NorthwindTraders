using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Northwind.Application.Interfaces;
using Northwind.Infrastructure;
using Northwind.Infrastructure.Claims;
using Northwind.Infrastructure.Identity.Interfaces;
using Northwind.Infrastructure.Identity.Models;
using Northwind.Infrastructure.Identity.Services;
using Northwind.Infrastructure.Interfaces;
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
                o.Password.RequireDigit = configuration.GetValue<bool>(ConfigKeys.IdentityPasswordRequireDigit);
                o.Password.RequireLowercase = configuration.GetValue<bool>(ConfigKeys.IdentityPasswordRequireLowercase);
                o.Password.RequireUppercase = configuration.GetValue<bool>(ConfigKeys.IdentityPasswordRequireUppercase);
                o.Password.RequireNonAlphanumeric = configuration.GetValue<bool>(ConfigKeys.IdentityPasswordNonAlphanumeric);
                o.User.RequireUniqueEmail = configuration.GetValue<bool>(ConfigKeys.IdentityUserRequireUniqueEmail);
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<AuditInterceptor>();
            services.AddScoped<IClaimManager, ClaimManager>();

            return services;
        }
    }
}
