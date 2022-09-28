using Microsoft.EntityFrameworkCore;
using Northwind.Domain.Common.Interfaces.Repositories;
using Northwind.Domain.Common.Models;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public class OrderDetailRepository : GenericRepository<OrderDetail, OrderDetailKey>, IOrderDetailRepository
    {
        public OrderDetailRepository(NorthwindContext context) : base(context)
        {
        }

        public override async Task<OrderDetail>? GetAsync(OrderDetailKey key)
        {
            return await _context.Set<OrderDetail>().FindAsync(key.OrderId, key.ProductId);
        }

        public async Task<IEnumerable<OrderDetail>>? GetAsync(OrderDetailKey[] keys)
        {
            var orderDetails = await NorthwindContext.OrderDetails.ToListAsync();
            var result = new List<OrderDetail>();

            foreach (var key in keys)
            {
                result.Add(orderDetails.Where(orderDetail => orderDetail.OrderId == key.OrderId && orderDetail.ProductId == key.ProductId).Single());
            }

            return result;
        }

        public NorthwindContext NorthwindContext
        {
            get { return _context as NorthwindContext; }
        }
    }
}
 