using System.ComponentModel.DataAnnotations;

namespace Northwind.Api.Models
{
    public class UserLoginRequest
    {
        [EmailAddress]
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
