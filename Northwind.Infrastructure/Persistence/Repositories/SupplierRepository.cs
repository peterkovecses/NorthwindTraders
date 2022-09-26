using Northwind.Domain.Common.Interfaces.Repositories;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public class SupplierRepository : GenericRepository<Supplier, int>, ISupplierRepository
    {
        public SupplierRepository(NorthwindContext northwindContext) : base(northwindContext)
        {
        }

        public NorthwindContext NorthwindContext
        {
            get { return _context as NorthwindContext; }
        }
    }
}
