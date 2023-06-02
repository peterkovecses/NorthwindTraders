using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Northwind.Infrastructure.Claims
{
    public static partial class AuthorizationClaims
    {
        public static Claim CustomerViewer => new("customers.view", "true");
        public static Claim CustomerWriter => new("customers.write", "true");
        public static IEnumerable<Claim> All => new List<Claim>()
        {
            CustomerViewer,
            CustomerWriter
        };

        public static IServiceCollection AddAuthorizationClaimPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(ClaimPolicies.CustomerViewer, builder =>
                    builder.RequireClaim(
                        AuthorizationClaims.CustomerViewer.Type,
                        AuthorizationClaims.CustomerViewer.Value));

                opt.AddPolicy(ClaimPolicies.CustomerAdministrator, builder =>
                {
                    builder.RequireClaim(AuthorizationClaims.CustomerViewer.Type, AuthorizationClaims.CustomerViewer.Value);
                    builder.RequireClaim(AuthorizationClaims.CustomerWriter.Type, AuthorizationClaims.CustomerWriter.Value);
                });
            });

            return services;
        }        
    }
}
