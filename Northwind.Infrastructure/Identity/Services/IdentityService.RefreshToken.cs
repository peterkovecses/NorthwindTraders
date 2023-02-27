using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Northwind.Application.Models;
using Northwind.Infrastructure.Exceptions;
using Northwind.Infrastructure.Identity.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Northwind.Infrastructure.Identity.Services
{
    public partial class IdentityService : IIdentityService
    {
        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            ClaimsPrincipal? validatedToken;
            try
            {
                validatedToken = GetPrincipalFromToken(token);
            }
            catch (InvalidJwtException ex)
            {
                return new AuthenticationResult { Errors = new[] { ex.Message } };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet." } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(r => r.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not exists." } };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has expired." } };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated." } };
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been used." } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT." } };
            }

            storedRefreshToken.Used = true;
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await CreateSuccessfulAuthenticationResultAsync(user);
        }

        private ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal? principal = default;
            SecurityToken validatedToken;

            var tokenValidationParameters = _tokenValidationParameters;

            try
            {
                principal = tokenHandler.ValidateToken(token, tokenValidationParameters.ToTokenValidationParameters(), out validatedToken);
            }
            catch
            {
                throw new InvalidJwtException();
            }

            if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
            {
                throw new InvalidJwtException();
            }
            return principal;
        }

        private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return validatedToken is JwtSecurityToken jwtSecurityToken &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
