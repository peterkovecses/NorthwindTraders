using LinqKit;
using Northwind.Application.Models.Filters;
using Northwind.Application.Models;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services.PredicateBuilders
{
    public class ProductPredicateBuilder
    {
        public virtual ExpressionStarter<Product> GetPredicate(QueryParameters<ProductFilter> queryParameters)
        {
            var predicate = PredicateBuilder.New<Product>(true);
            var filter = queryParameters.Filter;

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                predicate = predicate.And(p => p.ProductName.ToLower().Contains(filter.SearchTerm.ToLower()));
            }

            if (filter.SupplierId != null)
            {
                predicate = predicate.And(p => p.SupplierId == filter.SupplierId);
            }

            if (filter.CategoryId != null)
            {
                predicate = predicate.And(p => p.CategoryId == filter.CategoryId);
            }

            if (filter.MinUnitPrice != null)
            {
                predicate = predicate.And(p => p.UnitPrice >= filter.MinUnitPrice);
            }

            if (filter.MaxUnitPrice != null)
            {
                predicate = predicate.And(p => p.UnitPrice <= filter.MaxUnitPrice);
            }

            return predicate;
        }
    }
}
