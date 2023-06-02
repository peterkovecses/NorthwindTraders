using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Northwind.Application.Interfaces;
using Northwind.Application.Models;
using Northwind.Infrastructure.Claims;
using Northwind.Infrastructure.Identity.Models;
using Northwind.Infrastructure.Interfaces;
using Northwind.Infrastructure.Persistence;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Northwind.Infrastructure.Identity.Services
{
    public partial class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IClaimManager _claimManager;
        private readonly IdentityContext _context;
        private readonly IConfiguration _configuration;
        private readonly CustomTokenValidationParameters _tokenValidationParameters;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IClaimManager claimManager,
            IdentityContext context,
            IConfiguration configuration,
            IOptions<CustomTokenValidationParameters> options)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _claimManager = claimManager;
            _context = context;
            _configuration = configuration;
            _tokenValidationParameters = options.Value;
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

        public async Task AddRole(string role)
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }

        public async Task AddUserToRoles(string email, params string[] roles)
        {
            var user = await _userManager.FindByEmailAsync(email);
            await _userManager.AddToRolesAsync(user, roles);
        }

        public async Task AddClaimsToRole(IEnumerable<string> claimTypes, string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            foreach (var claim in _claimManager.FilterByTypes(claimTypes))
            {
                await _roleManager.AddClaimAsync(role, claim);
            }
        }

        public async Task AddClaimsToUser(IEnumerable<Claim> claims, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            await _userManager.AddClaimsAsync(user, claims);
        }

        private async Task<AuthenticationResult> CreateSuccessfulAuthenticationResultAsync(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration[ConfigKeys.TokenSecret]);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = (await _userManager.GetRolesAsync(user))
                                .Select(r => new Claim(ClaimTypes.Role, r));
            claims.AddRange(userRoles);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _tokenValidationParameters.ValidIssuer,
                Audience = _tokenValidationParameters.ValidAudience,
                Expires = DateTime.UtcNow.Add(_configuration.GetValue<TimeSpan>(ConfigKeys.TokenLifeTime)),
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
    }
}
