using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer, string>
    {
    }
}
