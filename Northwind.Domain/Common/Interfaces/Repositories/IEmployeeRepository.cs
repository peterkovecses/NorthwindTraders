using Northwind.Domain.Entities;

namespace Northwind.Domain.Common.Interfaces.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee, int>
    {
    }
}
