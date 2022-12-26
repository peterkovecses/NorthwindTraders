using Northwind.Application.Models;

namespace Northwind.Infrastructure.Identity.Models
{
    public class UserCreationResult : Result
    {
        public ApplicationUser? User { get; set; }
    }
}
