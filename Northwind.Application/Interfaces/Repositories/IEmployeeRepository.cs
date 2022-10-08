using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee, int>
    {
    }
}
