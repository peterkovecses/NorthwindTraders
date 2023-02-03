using Northwind.Application.Models;
using Northwind.Infrastructure.Claims;
using Northwind.Infrastructure.Identity.Interfaces;
using Northwind.Infrastructure.Identity.Models;

namespace Northwind.Infrastructure.Identity.Services
{
    public partial class IdentityService : IIdentityService
    {
        public async Task<Result> RegisterAsync(
            string email,
            string password,
            IEnumerable<string>? claimTypes = default,
            IEnumerable<string>? roles = default)
        {
            var errors = new List<string>();
            ValidateClaims(claimTypes, errors);
            await ValidateRoles(roles, errors);
            if (errors.Any())
            {
                return new Result { Errors = errors };
            }

            var userCreationResult = await CreateUserAsync(email, password);
            if (!userCreationResult.Success)
            {
                return new Result
                {
                    Errors = userCreationResult.Errors
                };
            }

            if (HasClaims(claimTypes))
            {
                await AddClaimsForUserAsync(userCreationResult.User, claimTypes);
            }

            if (HasRoles(roles))
            {
                await AddRolesForUserAsync(userCreationResult.User, roles);
            }

            return new Result
            {
                Success = true,
            };
        }

        private async Task ValidateRoles(IEnumerable<string>? roles, List<string> errors)
        {
            if (HasRoles(roles))
            {
                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        errors.Add($"{role} role not found.");
                    }
                }
            }
        }

        private void ValidateClaims(IEnumerable<string>? claimTypes, List<string> errors)
        {
            if (HasClaims(claimTypes))
            {
                var claimsValidationResult = _claimManager.AllClaimsExist(claimTypes);
                if (!claimsValidationResult.AllExists)
                {
                    errors.AddRange(claimsValidationResult.Errors);
                }
            }
        }

        private static bool HasRoles(IEnumerable<string>? roles)
        {
            return roles != null && roles.Any();
        }

        private static bool HasClaims(IEnumerable<string>? claimTypes)
        {
            return claimTypes != null && claimTypes.Any();
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
            var claims = AuthorizationClaims.All.Where(c => claimTypes.Contains(c.Type));
            await _userManager.AddClaimsAsync(user, claims);
        }

        private async Task AddRolesForUserAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
