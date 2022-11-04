using Microsoft.AspNetCore.Mvc;
using Northwind.Api.Errors;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

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
