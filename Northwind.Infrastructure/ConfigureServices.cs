﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Northwind.Application.Interfaces;
using Northwind.Infrastructure.Persistence;
using Northwind.Infrastructure.Persistence.Interceptors;
using Northwind.Infrastructure.Persistence.Services;
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

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<AuditInterceptor>();

            return services;
        }
    }
}
