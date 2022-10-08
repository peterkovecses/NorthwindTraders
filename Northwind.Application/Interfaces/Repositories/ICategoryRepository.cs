using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category, int>
    {
    }
}
