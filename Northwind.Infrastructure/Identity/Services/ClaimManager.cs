using Northwind.Application.Claims;
using Northwind.Infrastructure.Identity.Interfaces;
using Northwind.Infrastructure.Identity.Models;

namespace Northwind.Infrastructure.Identity.Services
{
    public class ClaimManager : IClaimManager
    {
        public bool ClaimExists(string claimType)
        {
            return ClaimsStore.AllClaims.Select(c => c.Type).Contains(claimType);
        }

        public ClaimsValidationResult AllClaimsExist(IEnumerable<string> claimTypes)
        {
            var claimTypesNotFound = claimTypes.Where(c => !ClaimsStore.AllClaims.Select(c => c.Type).Contains(c));
            if (claimTypesNotFound.Any())
            {
                var errors = claimTypesNotFound.Select(c => new string($"{c} claim not found."));
                return new ClaimsValidationResult { Errors = errors };
            }

            return new ClaimsValidationResult { AllExists = true };
        }
    }
}
