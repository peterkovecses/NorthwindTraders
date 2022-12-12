using AutoMapper;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Mappings;
using Northwind.Application.Services;

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

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICustomerDemographicService, CustomerDemographicService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();

            return services;
        }
    }
}
