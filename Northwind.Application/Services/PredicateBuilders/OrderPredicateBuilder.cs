using LinqKit;
using Northwind.Application.Models.Filters;
using Northwind.Application.Models;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services.PredicateBuilders
{
    public class OrderPredicateBuilder
    {
        public virtual ExpressionStarter<Order> GetPredicate(QueryParameters<OrderFilter> queryParameters)
        {
            var predicate = PredicateBuilder.New<Order>(true);
            var filter = queryParameters.Filter;

            if (filter.EmployeeId != null)
            {
                predicate = predicate.And(o => o.EmployeeId == filter.EmployeeId);
            }

            if (filter.MinOrderDate != null)
            {
                predicate = predicate.And(o => o.OrderDate >= filter.MinOrderDate);
            }

            if (filter.MaxOrderDate != null)
            {
                predicate = predicate.And(o => o.OrderDate <= filter.MaxOrderDate);
            }

            if (filter.MinRequiredDate != null)
            {
                predicate = predicate.And(o => o.RequiredDate >= filter.MinRequiredDate);
            }

            if (filter.MaxRequiredDate != null)
            {
                predicate = predicate.And(o => o.RequiredDate <= filter.MaxRequiredDate);
            }

            if (filter.MinShippedDate != null)
            {
                predicate = predicate.And(o => o.ShippedDate >= filter.MinShippedDate);
            }

            if (filter.MaxShippedDate != null)
            {
                predicate = predicate.And(o => o.ShippedDate <= filter.MaxShippedDate);
            }

            if (filter.ShipVia != null)
            {
                predicate = predicate.And(o => o.ShipVia == filter.ShipVia);
            }

            if (filter.MinFreight != null)
            {
                predicate = predicate.And(o => o.Freight >= filter.MinFreight);
            }

            if (filter.MaxFreight != null)
            {
                predicate = predicate.And(o => o.Freight <= filter.MaxFreight);
            }

            if (!string.IsNullOrEmpty(filter.ShipName))
            {
                predicate = predicate.And(o => o.ShipName == filter.ShipName);
            }

            if(!string.IsNullOrEmpty(filter.ShipCity))
            {
                predicate = predicate.And(o => o.ShipCity == filter.ShipCity);
            }

            if (!string.IsNullOrEmpty(filter.ShipRegion))
            {
                predicate = predicate.And(o => o.ShipRegion == filter.ShipRegion);
            }

            if (!string.IsNullOrEmpty(filter.ShipPostalCode))
            {
                predicate = predicate.And(o => o.ShipPostalCode == filter.ShipPostalCode);
            }

            if (!string.IsNullOrEmpty(filter.ShipCountry))
            {
                predicate = predicate.And(o => o.ShipCountry == filter.ShipCountry);
            }

            return predicate;
        }
    }
}
