using Northwind.Domain.Common.Interfaces.Repositories;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(NorthwindContext context) : base(context)
        {
        }

        public NorthwindContext NorthwindContext
        {
            get { return _context as NorthwindContext; }
        }
    }
}
