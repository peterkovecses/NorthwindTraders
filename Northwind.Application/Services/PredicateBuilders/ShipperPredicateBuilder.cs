using LinqKit;
using Northwind.Application.Models.Filters;
using Northwind.Application.Models;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services.PredicateBuilders
{
    public class ShipperPredicateBuilder
    {
        public virtual ExpressionStarter<Shipper> GetPredicate(QueryParameters<ShipperFilter> queryParameters)
        {
            var predicate = PredicateBuilder.New<Shipper>(true);
            var filter = queryParameters.Filter;

            if (!string.IsNullOrEmpty(filter.CompanyName))
            {
                predicate = predicate.And(s => s.CompanyName == s.CompanyName);
            }

            return predicate;
        }
    }
}
