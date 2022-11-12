using LinqKit;
using Northwind.Application.Models.Filters;
using Northwind.Application.Models;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services.PredicateBuilders
{
    public class RegionPredicateBuilder
    {
        public virtual ExpressionStarter<Region> GetPredicate(QueryParameters<RegionFilter> queryParameters)
        {
            var predicate = PredicateBuilder.New<Region>(true);
            var filter = queryParameters.Filter;

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                predicate = predicate.And(r => r.RegionDescription.ToLower().Contains(filter.SearchTerm.ToLower()));
            }

            return predicate;
        }
    }
}
