using System.ComponentModel.DataAnnotations;

namespace Northwind.Application.Models
{
    public class UserRegistrationRequest
    {
        [EmailAddress]
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public IEnumerable<string> ClaimTypes { get; set; } = default!;
    }
}
