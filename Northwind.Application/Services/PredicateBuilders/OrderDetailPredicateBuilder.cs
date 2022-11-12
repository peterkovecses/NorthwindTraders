using LinqKit;
using Northwind.Application.Models.Filters;
using Northwind.Application.Models;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services.PredicateBuilders
{
    public class OrderDetailPredicateBuilder
    {
        public virtual ExpressionStarter<OrderDetail> GetPredicate(QueryParameters<OrderDetailFilter> queryParameters)
        {
            var predicate = PredicateBuilder.New<OrderDetail>(true);
            var filter = queryParameters.Filter;

            if (filter.MinQuantity != null)
            {
                predicate = predicate.And(orderDetail => orderDetail.Quantity >= filter.MinQuantity);
            }

            if (filter.MaxQuantity != null)
            {
                predicate = predicate.And(orderDetail => orderDetail.Quantity <= filter.MaxQuantity);
            }

            return predicate;
        }
    }
}
