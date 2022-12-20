using Microsoft.AspNetCore.Authentication;
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

            services
                .AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, IdentityContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<AuditInterceptor>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddAuthorization(options =>
                options.AddPolicy("CanAddAdminUser", policy => policy.RequireRole("Administrator")));

            return services;
        }
    }
}
