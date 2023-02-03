using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Northwind.Api.Authorization;
using Northwind.Api.Errors;
using Northwind.Api.Services;
using Northwind.Application.Interfaces;
using Northwind.Infrastructure;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<ICurrentUserService, CurrentUserService>();

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

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = configuration.GetValue<bool>(ConfigKeys.TokenValidateIssuer),
                ValidateAudience = configuration.GetValue<bool>(ConfigKeys.TokenValidateAudience),
                ValidateLifetime = configuration.GetValue<bool>(ConfigKeys.TokenValidateLifetime),
                ValidateIssuerSigningKey = configuration.GetValue<bool>(ConfigKeys.TokenValidateIssuerSigningKey),
                ValidIssuer = configuration[ConfigKeys.TokenValidIssuer],
                ValidAudience = configuration[ConfigKeys.TokenValidAudience],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[ConfigKeys.TokenSecret]))
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme= JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("MustForMyCompany", policy =>
                policy.AddRequirements(new WorksForCompanyRequirement("comp.com")));
            });

            services.AddSingleton<IAuthorizationHandler, WorksforCompanyHandler>();

            services.AddSwaggerGen(opt =>
            {
                opt.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }
    }
}
