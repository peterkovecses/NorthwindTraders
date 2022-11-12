using LinqKit;
using Northwind.Application.Models.Filters;
using Northwind.Application.Models;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services.PredicateBuilders
{
    public class SupplierPredicateBuilder
    {
        public virtual ExpressionStarter<Supplier> GetPredicate(QueryParameters<SupplierFilter> queryParameters)
        {
            var predicate = PredicateBuilder.New<Supplier>(true);
            var filter = queryParameters.Filter;

            if (!string.IsNullOrEmpty(filter.CompanyName))
            {
                predicate = predicate.And(s => s.CompanyName == s.CompanyName);
            }

            if (!string.IsNullOrEmpty(filter.City))
            {
                predicate = predicate.And(s => s.City == s.City);
            }

            if (!string.IsNullOrEmpty(filter.Region))
            {
                predicate = predicate.And(s => s.Region == s.Region);
            }

            if (!string.IsNullOrEmpty(filter.PostalCode))
            {
                predicate = predicate.And(s => s.PostalCode == s.PostalCode);
            }

            if (!string.IsNullOrEmpty(filter.Country))
            {
                predicate = predicate.And(s => s.Country == s.Country);
            }

            return predicate;
        }
    }
}
