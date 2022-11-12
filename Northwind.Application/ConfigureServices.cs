using AutoMapper;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Mappings;
using Northwind.Application.Services;
using Northwind.Application.Services.PredicateBuilders;

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

            services.AddScoped<CategoryPredicateBuilder>();
            services.AddScoped<CustomerPredicateBuilder>();
            services.AddScoped<EmployeePredicateBuilder>();
            services.AddScoped<OrderDetailPredicateBuilder>();
            services.AddScoped<OrderPredicateBuilder>();
            services.AddScoped<ProductPredicateBuilder>();
            services.AddScoped<RegionPredicateBuilder>();
            services.AddScoped<ShipperPredicateBuilder>();
            services.AddScoped<SupplierPredicateBuilder>();
            services.AddScoped<TerritoryPredicateBuilder>();

            return services;
        }
    }
}
