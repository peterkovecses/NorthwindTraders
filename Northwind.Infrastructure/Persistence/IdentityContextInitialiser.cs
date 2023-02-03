using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Infrastructure.Claims;
using Northwind.Infrastructure.Identity.Interfaces;
using Northwind.Infrastructure.Identity.Models;

namespace Northwind.Infrastructure.Persistence
{
    public class IdentityContextInitialiser
    {
        private readonly ILogger<IdentityContextInitialiser> _logger;
        private readonly IdentityContext _context;
        private readonly IIdentityService _identityService;

        public IdentityContextInitialiser(
            ILogger<IdentityContextInitialiser> logger,
            IdentityContext context,
            IIdentityService identityService)
        {
            _logger = logger;
            _context = context;
            _identityService = identityService;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                if (_identityService.NoIdentityData())
                {
                    await TrySeedAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            var administratorRole = new IdentityRole("Administrator");
            var testerRole = new IdentityRole("Tester");
            await _identityService.AddRoles(administratorRole, testerRole);

            var administrator = new ApplicationUser { UserName = "admin@comp.com", Email = "admin@comp.com" };
            var tester = new ApplicationUser { UserName = "tester@comp.com", Email = "tester@comp.com" };
            await _identityService.AddUsers("Password11!", administrator, tester);

            await _identityService.AddUsersToRoles((administrator, new[] { administratorRole.Name }), (tester, new[] { testerRole.Name }));

            await _identityService.AddClaimsToRoles(AuthorizationClaims.All, administratorRole);
            await _identityService.AddClaimsToUsers(AuthorizationClaims.All, tester);
        }        
    }
}
