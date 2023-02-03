using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Northwind.Infrastructure.Claims
{
    public static class AuthorizationClaims
    {
        public static Claim CustomerViewer => new("customers.view", "true");
        public static Claim CustomerWriter => new("customers.write", "true");
        public static List<Claim> All => new()
        {
            CustomerViewer,
            CustomerWriter
        };

        public static class Policies
        {
            public const string CustomerViewer = "CustomerViewer";
            public const string CustomerAdministrator = "CustomerAdministrator";
        }

        public static IServiceCollection AddAuthorizationClaimPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(Policies.CustomerViewer, builder =>
                    builder.RequireClaim(
                        AuthorizationClaims.CustomerViewer.Type,
                        AuthorizationClaims.CustomerViewer.Value));

                opt.AddPolicy(Policies.CustomerAdministrator, builder =>
                {
                    builder.RequireClaim(AuthorizationClaims.CustomerViewer.Type, AuthorizationClaims.CustomerViewer.Value);
                    builder.RequireClaim(AuthorizationClaims.CustomerWriter.Type, AuthorizationClaims.CustomerWriter.Value);
                });
            });

            return services;
        }
    }
}
