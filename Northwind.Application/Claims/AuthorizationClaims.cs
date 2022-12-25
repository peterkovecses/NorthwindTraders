using System.Security.Claims;

namespace Northwind.Application.Claims
{
    public static class AuthorizationClaims
    {
        public static Claim CustomerViewer => new("customers.view", "true");
        public static Claim CustomerWriter => new("customers.write", "true");
    }
}
