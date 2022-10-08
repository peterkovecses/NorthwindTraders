using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order, int>
    {
    }
}
