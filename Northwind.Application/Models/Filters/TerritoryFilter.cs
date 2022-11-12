using Northwind.Application.Interfaces;

namespace Northwind.Application.Models.Filters
{
    public class TerritoryFilter : IFilter
    {
        public string? SearchTerm { get; set; }
        public int? RegionId { get; set; }
    }
}
