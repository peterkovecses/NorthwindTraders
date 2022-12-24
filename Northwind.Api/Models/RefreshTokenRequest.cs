namespace Northwind.Api.Models
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }
}
