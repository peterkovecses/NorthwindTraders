using LinqKit;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Repositories;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public class OrderDetailRepository : GenericRepository<OrderDetail, IOrderDetailKey>, IOrderDetailRepository
    {
        public OrderDetailRepository(NorthwindContext context) : base(context)
        {
        }

        public override async Task<OrderDetail?> FindByIdAsync(IOrderDetailKey key, CancellationToken token = default)
        {
            return await _context.Set<OrderDetail>().FindAsync(new object?[] { key.ProductId, key.OrderId }, cancellationToken: token);
        }

        public async Task<IEnumerable<OrderDetail>> FindByIdsAsync(IOrderDetailKey[] keys, CancellationToken token)
        {
            var predicate = PredicateBuilder.New<OrderDetail>();

            foreach (var key in keys)
            {
                predicate = predicate.Or(orderDetail => orderDetail.OrderId == key.OrderId && orderDetail.ProductId == key.ProductId);
            }

            return await NorthwindContext.OrderDetails.AsExpandable().Where(predicate).ToListAsync(token);
        }

        public NorthwindContext NorthwindContext
        {
            get { return _context as NorthwindContext; }
        }
    }
}
 