using Northwind.Domain.Common.Interfaces.Repositories;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public class TerritoryRepository : GenericRepository<Territory>, ITerritoryRepository
    {
        public TerritoryRepository(NorthwindContext context) : base(context)
        {
        }

        public NorthwindContext NorthwindContext
        {
            get { return _context as NorthwindContext; }
        }
    }
}
