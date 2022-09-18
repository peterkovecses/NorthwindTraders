using Northwind.Domain.Common.Interfaces;
using Northwind.Domain.Common.Interfaces.Repositories;
using Northwind.Infrastructure.Persistence.Repositories;

namespace Northwind.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NorthwindContext _context;

        public UnitOfWork(NorthwindContext context)
        {
            _context = context;
            Employees = new EmployeeRepository(_context);
        }

        public IEmployeeRepository Employees { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
