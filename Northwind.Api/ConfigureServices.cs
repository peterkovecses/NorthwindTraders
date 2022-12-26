using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Northwind.Api.Authorization;
using Northwind.Api.Errors;
using Northwind.Api.Policies;
using Northwind.Api.Services;
using Northwind.Application.Claims;
using Northwind.Application.Interfaces;
using Northwind.Application.Options;
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

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.ValidIssuer,
                ValidAudience = jwtOptions.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
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
                opt.AddPolicy(AuthorizationPolicies.CustomerViewer, builder => 
                    builder.RequireClaim(
                        AuthorizationClaims.CustomerViewer.Type, 
                        AuthorizationClaims.CustomerViewer.Value));

                opt.AddPolicy(AuthorizationPolicies.CustomerAdministrator, builder =>
                {
                    builder.RequireClaim(AuthorizationClaims.CustomerViewer.Type, AuthorizationClaims.CustomerViewer.Value);
                    builder.RequireClaim(AuthorizationClaims.CustomerWriter.Type, AuthorizationClaims.CustomerWriter.Value);
                });

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
