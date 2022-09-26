using AutoMapper;
using Microsoft.AspNetCore.Http;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Mappings;
using Northwind.Application.Common.Services;
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

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IPaginatedUriService>(provider =>
            {
                var acessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = acessor.HttpContext.Request;
                var absoluteUri = string.Concat($"{request.Scheme}://{request.Host.ToUriComponent()}{request.Path}", "/");
                return new PaginatedUriService(absoluteUri);
            });

            return services;
        }
    }
}
