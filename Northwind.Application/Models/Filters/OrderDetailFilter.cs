using LinqKit;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Application.Models.Filters
{
    public class OrderDetailFilter : IFilter
    {
        public short? MinQuantity { get; set; }
        public short? MaxQuantity { get; set; }

        public ExpressionStarter<OrderDetail> GetPredicate()
        {
            var predicate = PredicateBuilder.New<OrderDetail>(true);

            if (MinQuantity != null)
            {
                predicate = predicate.And(orderDetail => orderDetail.Quantity >= MinQuantity);
            }

            if (MaxQuantity != null)
            {
                predicate = predicate.And(orderDetail => orderDetail.Quantity <= MaxQuantity);
            }

            return predicate;
        }
    }
}
