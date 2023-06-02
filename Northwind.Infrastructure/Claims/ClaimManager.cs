using Northwind.Infrastructure.Interfaces;
using System.Security.Claims;

namespace Northwind.Infrastructure.Claims
{
    public class ClaimManager : IClaimManager
    {
        public bool ClaimExists(string claimType)
            => AuthorizationClaims.All.Select(claim => claim.Type).Contains(claimType);

        public IEnumerable<Claim> FilterByTypes(IEnumerable<string> claimTypes)
            => AuthorizationClaims.All.Where(claim => claimTypes.Contains(claim.Type));

        public ClaimsValidationResult AllClaimsExist(IEnumerable<string> claimTypes)
        {
            var claimTypesNotFound = claimTypes.Except(FilterByTypes(claimTypes).Select(claim => claim.Type));

            if (claimTypesNotFound.Any())
            {
                var errors = claimTypesNotFound.Select(claimType => $"{claimType} claim not found.");
                return new ClaimsValidationResult { Errors = errors };
            }

            return new ClaimsValidationResult { AllExists = true };
        }
    }
}
