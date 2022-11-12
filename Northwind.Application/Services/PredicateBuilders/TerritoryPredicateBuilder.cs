using LinqKit;
using Northwind.Application.Models.Filters;
using Northwind.Application.Models;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services.PredicateBuilders
{
    public class TerritoryPredicateBuilder
    {
        public virtual ExpressionStarter<Territory> GetPredicate(QueryParameters<TerritoryFilter> queryParameters)
        {
            var predicate = PredicateBuilder.New<Territory>(true);
            var filter = queryParameters.Filter;

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                predicate = predicate.And(t => t.TerritoryDescription.ToLower().Contains(filter.SearchTerm.ToLower()));
            }

            if (filter.RegionId != null)
            {
                predicate = predicate.And(r => r.RegionId == filter.RegionId);
            }

            return predicate;
        }
    }
}
