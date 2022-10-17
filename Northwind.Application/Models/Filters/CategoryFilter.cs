using Northwind.Application.Interfaces;

namespace Northwind.Application.Models.Filters
{
    public class CategoryFilter : IFilter
    {
        public string SearchTerm { get; set; } = null!;
    }
}
