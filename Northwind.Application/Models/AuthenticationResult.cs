namespace Northwind.Application.Models
{
    public class AuthenticationResult : Result
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}