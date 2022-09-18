using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Mappings;
using Northwind.Application.Services;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IEmployeeService, EmployeeService>();

            return services;
        }
    }
}
