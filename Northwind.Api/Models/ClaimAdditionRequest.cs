namespace Northwind.Api.Models
{
    public class ClaimAdditionRequest
    {
        public string Username { get; set; } = default!;
        public IEnumerable<string> ClaimTypes { get; set; } = default!;
    }
}
