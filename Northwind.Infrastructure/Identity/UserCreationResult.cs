using Northwind.Application.Models;
using Northwind.Infrastructure.Identity;

namespace Northwind.Infrastructure.Identity
{
    public class UserCreationResult : Result
    {
        public ApplicationUser? User { get; set; }
    }
}
