using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Northwind.Application.Models;
using Northwind.Infrastructure.Identity.Interfaces;
using Northwind.Infrastructure.Identity.Models;
using Northwind.Infrastructure.Interfaces;
using Northwind.Infrastructure.Persistence;
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
        private readonly TokenValidationParameters _tokenValidationParameters;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IClaimManager claimManager,
            IdentityContext context,
            IConfiguration configuration,
            TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _claimManager = claimManager;
            _context = context;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
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

        public bool NoIdentityData()
        {
            return !(_roleManager.Roles.Any() || _userManager.Users.Any());
        }

        public async Task AddRoles(params IdentityRole[] roles)
        {
            foreach (var role in roles)
            {
                await _roleManager.CreateAsync(role);
            }
        }

        public async Task AddUsers(string password, params ApplicationUser[] users)
        {
            foreach (var user in users)
            {
                await _userManager.CreateAsync(user, password);
            }
        }

        public async Task AddUsersToRoles(params (ApplicationUser user, IEnumerable<string> roles)[] userRolePairs)
        {
            foreach (var (user, roles) in userRolePairs)
            {
                await _userManager.AddToRolesAsync(user, roles);
            }
        }

        public async Task AddClaimsToRoles(IEnumerable<Claim> claims, params IdentityRole[] administratorRoles)
        {
            foreach (var claim in claims)
            {
                foreach (var role in administratorRoles)
                {
                    await _roleManager.AddClaimAsync(role, claim);
                }
            }
        }

        public async Task AddClaimsToUsers(IEnumerable<Claim> claims, params ApplicationUser[] users)
        {
            foreach (var user in users)
            {
                await _userManager.AddClaimsAsync(user, claims);
            }
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
                Issuer = _configuration[ConfigKeys.TokenValidIssuer],
                Audience = _configuration[ConfigKeys.TokenValidAudience],
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
