using Northwind.Domain.Common.Interfaces.Repositories;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(NorthwindContext context) : base(context)
        {
        }

        public NorthwindContext NorthwindContext
        {
            get { return _context as NorthwindContext; }
        }
    }
}
