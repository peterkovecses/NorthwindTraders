using Northwind.Application.Interfaces;

namespace Northwind.Application.Models.Filters
{
    public class OrderDetailFilter : IFilter
    {
        public short MinQuantity { get; set; }
        public short MaxQuantity { get; set; }
    }
}
