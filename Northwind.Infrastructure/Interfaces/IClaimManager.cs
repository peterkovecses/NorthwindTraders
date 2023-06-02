using Northwind.Infrastructure.Claims;
using System.Security.Claims;

namespace Northwind.Infrastructure.Interfaces
{
    public interface IClaimManager
    {
        bool ClaimExists(string claimType);
        ClaimsValidationResult AllClaimsExist(IEnumerable<string> claimTypes);
        IEnumerable<Claim> FilterByTypes(IEnumerable<string> claimTypes);
    }
}
