using Northwind.Application.Interfaces;

namespace Northwind.Application.Models.Filters
{
    public class ShipperFilter : IFilter
    {
        public string CompanyName { get; set; } = null!;
    }
}
