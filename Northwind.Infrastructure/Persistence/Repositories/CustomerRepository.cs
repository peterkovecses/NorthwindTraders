using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Repositories;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : GenericRepository<Customer, string>, ICustomerRepository
    {
        public CustomerRepository(NorthwindContext context, IStrategyResolver strategyResolver) : base(context, strategyResolver)
        {
        }

        public NorthwindContext NorthwindContext
        {
            get { return _context as NorthwindContext; }
        }
    }
}
