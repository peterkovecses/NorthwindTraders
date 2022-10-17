using Northwind.Application.Interfaces;

namespace Northwind.Application.Models.Filters
{
    public class CustomerFilter : IFilter
    {
        public string? SearchTerm { get; set; }
        public string? CompanyName { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
    }
}
