using Northwind.Application.Interfaces;

namespace Northwind.Application.Models.Filters
{
    public class ProductFilter : IFilter
    {
        public string? SearchTerm { get; set; }
        public int? SupplierId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinUnitPrice { get; set; }
        public decimal? MaxUnitPrice { get; set; }
    }
}
