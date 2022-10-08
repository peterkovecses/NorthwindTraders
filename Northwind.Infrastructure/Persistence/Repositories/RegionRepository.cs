using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Repositories;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public class RegionRepository : GenericRepository<Region, int>, IRegionRepository
    {
        public RegionRepository(NorthwindContext context, IStrategyResolver strategyResolver) : base(context, strategyResolver)
        {
        }

        public NorthwindContext NorthwindContext
        {
            get { return _context as NorthwindContext; }
        }
    }
}
