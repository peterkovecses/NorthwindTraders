using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Northwind.Api.Errors;
using Northwind.Api.Options;
using Northwind.Api.Services;
using Northwind.Application.Interfaces;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<ICurrentUserService, CurrentUserService>();

            var jwtOptions = new JwtOptions();
            configuration.Bind(nameof(jwtOptions), jwtOptions);
            services.AddSingleton(jwtOptions);

            services.AddControllers();
            services.AddEndpointsApiExplorer();

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

            services
                .AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opt =>
                {
                    opt.SaveToken = true;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Secret)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = false,
                        ValidateLifetime = true
                    };
                });

            services.AddSwaggerGen(opt =>
            {
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", Array.Empty<string>() }
                };

                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
            });

            return services;
        }
    }
}
