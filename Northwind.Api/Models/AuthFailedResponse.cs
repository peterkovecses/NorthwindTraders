namespace Northwind.Application.Models
{
    public class AuthFailedResponse
    {
        public IEnumerable<string> Errors { get; set; } = default!;
    }
}
