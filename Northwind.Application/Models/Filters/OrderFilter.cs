using Northwind.Application.Interfaces;

namespace Northwind.Application.Models.Filters
{
    public class OrderFilter : IFilter
    {
        public int? EmployeeId { get; set; }
        public DateTime? MinOrderDate { get; set; }
        public DateTime? MaxOrderDate { get; set; }
        public DateTime? MinRequiredDate { get; set; }
        public DateTime? MaxRequiredDate { get; set; }
        public DateTime? MinShippedDate { get; set; }
        public DateTime? MaxShippedDate { get; set; }
        public int? ShipVia { get; set; }
        public decimal? MinFreight { get; set; }
        public decimal? MaxFreight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
    }
}
