using Northwind.Application.Common.Queries;

namespace Northwind.Application.Common.Interfaces
{
    public interface IGenericService<TEntity, TId> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(PaginationQuery? paginationQuery = null);
        Task<TEntity>? GetAsync(TId id);
        Task<TId> CreateAsync(TEntity obj);
        Task UpdateAsync(TEntity obj);
        Task<TEntity> DeleteAsync(TId id);
        Task<IEnumerable<TEntity>> DeleteRangeAsync(TId[] ids);
        Task<bool> IsExists(TId id);
        Task<bool> AreExists(TId[] id);
    }
}
