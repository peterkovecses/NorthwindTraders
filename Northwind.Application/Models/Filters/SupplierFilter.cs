using Northwind.Application.Interfaces;

namespace Northwind.Application.Models.Filters
{
    public class SupplierFilter : IFilter
    {
        public string? CompanyName { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
    }
}
