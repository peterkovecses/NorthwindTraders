using Northwind.Application.Models;

namespace Northwind.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(
            string email,
            string password,
            IEnumerable<string>? claimTypes = default,
            IEnumerable<string>? roles = default);
        Task<AuthenticationResult> LoginAsync(string email, string password);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
    }
}
