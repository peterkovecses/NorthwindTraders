using Microsoft.AspNetCore.Identity;
using Northwind.Application.Models;
using Northwind.Infrastructure.Identity.Models;
using System.Security.Claims;

namespace Northwind.Infrastructure.Identity.Interfaces
{
    public interface IIdentityService
    {
        Task<Result> RegisterAsync(
            string email,
            string password,
            IEnumerable<string>? claimTypes = default,
            IEnumerable<string>? roles = default);
        Task<AuthenticationResult> LoginAsync(string email, string password);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
        bool NoIdentityData();
        Task AddRoles(params IdentityRole[] roles);
        Task AddUsers(string password, params ApplicationUser[] users);
        Task AddUsersToRoles(params (ApplicationUser user, IEnumerable<string> roles)[] userRolePairs);
        Task AddClaimsToRoles(IEnumerable<Claim> claims, params IdentityRole[] administratorRoles);
        Task AddClaimsToUsers(IEnumerable<Claim> claims, params ApplicationUser[] users);
    }
}
