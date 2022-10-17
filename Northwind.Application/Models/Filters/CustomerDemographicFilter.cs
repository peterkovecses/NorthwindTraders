using Northwind.Application.Interfaces;

namespace Northwind.Application.Models.Filters
{
    public class CustomerDemographicFilter : IFilter
    {
        public string? SearchTerm { get; set; } = null!;
    }
}
