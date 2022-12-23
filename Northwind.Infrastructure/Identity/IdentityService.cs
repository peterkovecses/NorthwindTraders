using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Northwind.Application.Interfaces;
using Northwind.Application.Models;
using Northwind.Application.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Northwind.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _jwtOptions;

        public IdentityService(UserManager<ApplicationUser> userManager, JwtOptions jwtOptions)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions;
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByNameAsync(email);

            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with this e-mail address already exists." }
                };
            }

            var newUser = new ApplicationUser
            {
                Email = email,
                UserName = email
            };

            var createdUser = await _userManager.CreateAsync(newUser, password);

            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(e => e.Description)
                };
            }

            return CreateSuccessfulAuthenticationResult(newUser);
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

            return CreateSuccessfulAuthenticationResult(user);
        }

        private AuthenticationResult CreateSuccessfulAuthenticationResult(ApplicationUser newUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, newUser.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, newUser.Email),
                    new Claim("id", newUser.Id)
                }),
                Issuer = _jwtOptions.ValidIssuer,
                Audience = _jwtOptions.ValidAudience,
                Expires = DateTime.UtcNow.AddHours(_jwtOptions.expiresIn),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescription);

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token)
            };
        }
    }
}
