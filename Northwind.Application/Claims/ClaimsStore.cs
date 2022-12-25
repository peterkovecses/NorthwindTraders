using System.Security.Claims;

namespace Northwind.Application.Claims
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims => new()
        {
            AuthorizationClaims.CustomerViewer,
            AuthorizationClaims.CustomerWriter
        };
    }
}
