using Microsoft.EntityFrameworkCore;
using Northwind.Domain.Common.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Persistence.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly NorthwindContext _context;

        public EmployeeRepository(NorthwindContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee>? GetByIdAsync(int id)
        {
            return await _context.Employees.Where(e => e.EmployeeId == id).SingleOrDefaultAsync();
        }
        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public void Remove(Employee employee)
        {
            _context.Employees.Remove(employee);
        }

        public void RemoveRange(IEnumerable<Employee> employees)
        {
            _context.Employees.RemoveRange(employees);
        }
    }
}
