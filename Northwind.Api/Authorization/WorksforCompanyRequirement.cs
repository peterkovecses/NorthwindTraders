using Microsoft.AspNetCore.Authorization;

namespace Northwind.Api.Authorization
{
    public class WorksForCompanyRequirement : IAuthorizationRequirement
    {
        public string DomainName { get; }

        public WorksForCompanyRequirement(string domainName)
        {
            DomainName = domainName;
        }
    }
}
