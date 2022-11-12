using LinqKit;
using Northwind.Application.Models.Filters;
using Northwind.Application.Models;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services.PredicateBuilders
{
    public class CategoryPredicateBuilder
    {
        public virtual ExpressionStarter<Category> GetPredicate(QueryParameters<CategoryFilter> queryParameters)
        {
            var predicate = PredicateBuilder.New<Category>(true);
            var filter = queryParameters.Filter;

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                predicate = predicate.And(c => c.CategoryName.ToLower().Contains(filter.SearchTerm.ToLower()));
            }

            return predicate;
        }
    }
}
