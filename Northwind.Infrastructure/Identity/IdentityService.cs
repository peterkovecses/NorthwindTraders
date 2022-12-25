﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Northwind.Application.Claims;
using Northwind.Application.Interfaces;
using Northwind.Application.Models;
using Northwind.Application.Options;
using Northwind.Infrastructure.Exceptions;
using Northwind.Infrastructure.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Northwind.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _jwtOptions;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IdentityContext _context;

        public IdentityService(
            UserManager<ApplicationUser> userManager,            
            JwtOptions jwtOptions,
            TokenValidationParameters tokenValidationParameters,
            IdentityContext context)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
        }

        public async Task<AuthenticationResult> RegisterAsync(
            string email, 
            string password,
            IEnumerable<string>? claimTypes = default,
            IEnumerable<string>? roleNames = default)
        {
            bool hasClaims = claimTypes != null && claimTypes.Any();
            if (hasClaims)
            {
                var claimsValidationResult = ValidateClaims(claimTypes);
                if (!claimsValidationResult.Success)
                {
                    return new AuthenticationResult
                    {
                        Errors = claimsValidationResult.Errors
                    };
                }
            }

            var userCreationResult = await CreateUserAsync(email, password);

            if (!userCreationResult.Success)
            {
                return new AuthenticationResult
                {
                    Errors = userCreationResult.Errors
                };
            }            

            if (hasClaims)
            {
                await AddClaimsForUserAsync(userCreationResult.User, claimTypes);
            }
            
            return await CreateSuccessfulAuthenticationResultAsync(userCreationResult.User);
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByNameAsync(email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Incorrect user or password." }
                };
            }

            return await CreateSuccessfulAuthenticationResultAsync(user);
        }

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

        private static Result ValidateClaims(IEnumerable<string> claimTypes)
        {
            var claimTypesNotFound = claimTypes.Where(c => !ClaimsStore.AllClaims.Select(c => c.Type).Contains(c));
            if (claimTypesNotFound.Any())
            {
                var errors = claimTypesNotFound.Select(c => new string($"{c} claim not found."));
                return new Result
                {
                    Errors = errors
                };
            }

            return new Result
            {
                Success = true
            };
        }

        private async Task<UserCreationResult> CreateUserAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByNameAsync(email);

            if (existingUser != null)
            {
                return new UserCreationResult
                {
                    Errors = new[] { "User with this e-mail address already exists." }
                };
            }

            var newUserId = Guid.NewGuid();
            var newUser = new ApplicationUser
            {
                Id = newUserId.ToString(),
                Email = email,
                UserName = email
            };

            var result = await _userManager.CreateAsync(newUser, password);

            if (!result.Succeeded)
            {
                return new UserCreationResult
                {
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            return new UserCreationResult
            {
                Success = true,
                User = newUser
            };
        }

        private async Task AddClaimsForUserAsync(ApplicationUser user, IEnumerable<string> claimTypes)
        {
            var claims = ClaimsStore.AllClaims.Where(c => claimTypes.Contains(c.Type));
            await _userManager.AddClaimsAsync(user, claims);
        }

        private async Task<AuthenticationResult> CreateSuccessfulAuthenticationResultAsync(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);

            claims.AddRange(userClaims);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtOptions.ValidIssuer,
                Audience = _jwtOptions.ValidAudience,
                Expires = DateTime.UtcNow.Add(_jwtOptions.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescription);

            var refreshToken = new CustomRefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }

        private ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal? principal = default;
            SecurityToken validatedToken;
            try
            {
                principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out validatedToken);

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
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
