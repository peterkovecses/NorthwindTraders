using System.Linq.Expressions;

namespace Northwind.Application.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity, TId> where TEntity : class
    {
        Task<(int totalItems, IEnumerable<TEntity> items)> GetAsync(IPaginationQuery? paginationQuery = null, Expression<Func<TEntity, bool>>? predicate = null);
        Task<TEntity>? FindByIdAsync(TId id);
        Task AddAsync(TEntity entity);
        void Remove(IEnumerable<TEntity> entities);
    }
}
