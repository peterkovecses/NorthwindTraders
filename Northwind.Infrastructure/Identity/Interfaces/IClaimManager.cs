using Northwind.Infrastructure.Identity.Models;

namespace Northwind.Infrastructure.Identity.Interfaces
{
    public interface IClaimManager
    {
        bool ClaimExists(string claimType);
        ClaimsValidationResult AllClaimsExist(IEnumerable<string> claimTypes);
    }
}
