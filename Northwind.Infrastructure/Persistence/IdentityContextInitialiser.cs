using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Application.Interfaces;
using Northwind.Infrastructure.Claims;
using System.Security.Claims;

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
                if (!_context.Users.Any())
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

        private async Task TrySeedAsync()
        {
            await _identityService.AddRole("Administrator");
            await _identityService.AddRole("Tester");

            await _identityService.RegisterAsync("admin@comp.com", "Password11!", roles: new List<string> { "Administrator", });
            await _identityService.RegisterAsync("tester@comp.com", "Password11!", roles: new List<string> { "Tester", });

            await _identityService.AddClaimsToRole(AuthorizationClaims.All.Select(claim => claim.Type), "Administrator");
            await _identityService.AddClaimsToUser(new List<Claim> { AuthorizationClaims.CustomerViewer }, "tester@comp.com");
        }
    }
}
