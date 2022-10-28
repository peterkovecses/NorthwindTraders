using Microsoft.AspNetCore.Mvc;
using Northwind.Api.Common;
using Northwind.Api.Services;
using Northwind.Application.Interfaces.Services;

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

            services.AddMvc()
                .ConfigureApiBehaviorOptions(opt
                    =>
                    {
                        opt.InvalidModelStateResponseFactory = context =>
                        {
                            var problemDetails = new CustomBadRequest(context);

                            return new BadRequestObjectResult(problemDetails)
                            {
                                ContentTypes = { "application / problem + json", "application / problem + xml" }
                            };
                        };
                    });

            return services;
        }
    }
}
