using LinqKit;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Repositories;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public class OrderDetailRepository : GenericRepository<OrderDetail, IOrderDetailKey>, IOrderDetailRepository
    {
        public OrderDetailRepository(NorthwindContext context, IStrategyResolver strategyResolver) : base(context, strategyResolver)
        {
        }

        public override async Task<OrderDetail>? FindByIdAsync(IOrderDetailKey key)
        {
            return await _context.Set<OrderDetail>().FindAsync(key.OrderId, key.ProductId);
        }

        public async Task<IEnumerable<OrderDetail>> FindByIdsAsync(IOrderDetailKey[] keys)
        {
            var predicate = PredicateBuilder.New<OrderDetail>();

            foreach (var key in keys)
            {
                predicate = predicate.Or(orderDetail => orderDetail.OrderId == key.OrderId && orderDetail.ProductId == key.ProductId);
            }

            return await NorthwindContext.OrderDetails.AsExpandable().Where(predicate).ToListAsync();
        }

        public NorthwindContext NorthwindContext
        {
            get { return _context as NorthwindContext; }
        }
    }
}
 