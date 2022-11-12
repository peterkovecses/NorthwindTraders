using LinqKit;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

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

        public ExpressionStarter<Order> GetPredicate()
        {
            var predicate = PredicateBuilder.New<Order>(true);

            if (EmployeeId != null)
            {
                predicate = predicate.And(o => o.EmployeeId == EmployeeId);
            }

            if (MinOrderDate != null)
            {
                predicate = predicate.And(o => o.OrderDate >= MinOrderDate);
            }

            if (MaxOrderDate != null)
            {
                predicate = predicate.And(o => o.OrderDate <= MaxOrderDate);
            }

            if (MinRequiredDate != null)
            {
                predicate = predicate.And(o => o.RequiredDate >= MinRequiredDate);
            }

            if (MaxRequiredDate != null)
            {
                predicate = predicate.And(o => o.RequiredDate <= MaxRequiredDate);
            }

            if (MinShippedDate != null)
            {
                predicate = predicate.And(o => o.ShippedDate >= MinShippedDate);
            }

            if (MaxShippedDate != null)
            {
                predicate = predicate.And(o => o.ShippedDate <= MaxShippedDate);
            }

            if (ShipVia != null)
            {
                predicate = predicate.And(o => o.ShipVia == ShipVia);
            }

            if (MinFreight != null)
            {
                predicate = predicate.And(o => o.Freight >= MinFreight);
            }

            if (MaxFreight != null)
            {
                predicate = predicate.And(o => o.Freight <= MaxFreight);
            }

            if (!string.IsNullOrEmpty(ShipName))
            {
                predicate = predicate.And(o => o.ShipName == ShipName);
            }

            if (!string.IsNullOrEmpty(ShipCity))
            {
                predicate = predicate.And(o => o.ShipCity == ShipCity);
            }

            if (!string.IsNullOrEmpty(ShipRegion))
            {
                predicate = predicate.And(o => o.ShipRegion == ShipRegion);
            }

            if (!string.IsNullOrEmpty(ShipPostalCode))
            {
                predicate = predicate.And(o => o.ShipPostalCode == ShipPostalCode);
            }

            if (!string.IsNullOrEmpty(ShipCountry))
            {
                predicate = predicate.And(o => o.ShipCountry == ShipCountry);
            }

            return predicate;
        }
    }
}
