using Northwind.Infrastructure.Interfaces;

namespace Northwind.Infrastructure.Claims
{
    public class ClaimManager : IClaimManager
    {
        public bool ClaimExists(string claimType)
        {
            return AuthorizationClaims.All.Select(c => c.Type).Contains(claimType);
        }

        public ClaimsValidationResult AllClaimsExist(IEnumerable<string> claimTypes)
        {
            var claimTypesNotFound = claimTypes.Where(c => !AuthorizationClaims.All.Select(c => c.Type).Contains(c));
            if (claimTypesNotFound.Any())
            {
                var errors = claimTypesNotFound.Select(c => new string($"{c} claim not found."));
                return new ClaimsValidationResult { Errors = errors };
            }

            return new ClaimsValidationResult { AllExists = true };
        }
    }
}
