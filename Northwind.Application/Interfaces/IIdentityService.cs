using Northwind.Application.Models;
using System.Security.Claims;

namespace Northwind.Application.Interfaces
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
        Task AddRole(string role);
        Task AddUserToRoles(string email, params string[] roles);
        Task AddClaimsToRole(IEnumerable<string> claimTypes, string roleName);
        Task AddClaimsToUser(IEnumerable<Claim> claims, string email);
    }
}
