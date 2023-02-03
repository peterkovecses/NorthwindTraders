using Northwind.Infrastructure.Claims;

namespace Northwind.Infrastructure.Interfaces
{
    public interface IClaimManager
    {
        bool ClaimExists(string claimType);
        ClaimsValidationResult AllClaimsExist(IEnumerable<string> claimTypes);
    }
}
