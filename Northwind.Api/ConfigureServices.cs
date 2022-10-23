using Northwind.Application.Interfaces.Services;
using Northwind.Application.Services;
using Serilog;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

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
