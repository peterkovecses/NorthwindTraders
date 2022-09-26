using Northwind.Domain.Common.Queries;
using System.Linq.Expressions;

namespace Northwind.Domain.Common.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity, TId> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(PaginationFilter? paginationFilter = null);
        Task<TEntity>? GetAsync(TId id);
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, PaginationFilter? paginationFilter = null);
        Task<TEntity?> FindSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
