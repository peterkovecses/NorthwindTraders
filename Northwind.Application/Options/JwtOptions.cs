namespace Northwind.Application.Options
{
    public class JwtOptions
    {
        public string Secret { get; set; } = default!;
        public string ValidIssuer { get; set; } = default!;
        public string ValidAudience { get; set; } = default!;
        public int expiresIn { get; set; }
    }
}
