using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product, int>
    {
    }
}
