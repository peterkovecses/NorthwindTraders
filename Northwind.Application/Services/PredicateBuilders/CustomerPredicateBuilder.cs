using LinqKit;
using Northwind.Application.Models.Filters;
using Northwind.Application.Models;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services.PredicateBuilders
{
    public class CustomerPredicateBuilder
    {
        public virtual ExpressionStarter<Customer> GetPredicate(QueryParameters<CustomerFilter> queryParameters)
        {
            var predicate = PredicateBuilder.New<Customer>(true);
            var filter = queryParameters.Filter;

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                predicate = predicate.And(c => c.CompanyName.ToLower().Contains(filter.SearchTerm.ToLower()));
            }

            if (!string.IsNullOrEmpty(filter.City))
            {
                predicate = predicate.And(c => c.City == filter.City);
            }

            if (!string.IsNullOrEmpty(filter.Region))
            {
                predicate = predicate.And(c => c.Region == filter.Region);
            }

            if (!string.IsNullOrEmpty(filter.PostalCode))
            {
                predicate = predicate.And(c => c.PostalCode == filter.PostalCode);
            }

            if (!string.IsNullOrEmpty(filter.Country))
            {
                predicate = predicate.And(c => c.Country == filter.Country);
            }

            return predicate;
        }
    }
}
