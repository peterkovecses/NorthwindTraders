using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Application.Claims;
using Northwind.Infrastructure.Identity.Models;

namespace Northwind.Infrastructure.Persistence
{
    public class IdentityContextInitialiser
    {
        private readonly ILogger<IdentityContextInitialiser> _logger;
        private readonly IdentityContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityContextInitialiser(
            ILogger<IdentityContextInitialiser> logger,
            IdentityContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
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
                await TrySeedAsync();
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

            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(administratorRole);
                await _roleManager.CreateAsync(testerRole);
            }

            var administrator = new ApplicationUser { UserName = "admin@comp.com", Email = "admin@comp.com" };
            var tester = new ApplicationUser { UserName = "tester@comp.com", Email = "tester@comp.com" };

            if (!_userManager.Users.Any())
            {
                await _userManager.CreateAsync(administrator, "Password11!");
                await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });

                await _userManager.CreateAsync(tester, "Password");
                await _userManager.AddToRolesAsync(tester, new[] { testerRole.Name });

                var claims = ClaimsStore.AllClaims;
                await _userManager.AddClaimsAsync(administrator, claims);
                await _userManager.AddClaimsAsync(tester, claims);
            }
        }
    }
}
