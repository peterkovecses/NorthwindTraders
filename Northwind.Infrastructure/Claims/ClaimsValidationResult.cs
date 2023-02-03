namespace Northwind.Infrastructure.Claims
{
    public class ClaimsValidationResult
    {
        public bool AllExists { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
