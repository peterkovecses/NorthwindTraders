using Northwind.Application.Common.Queries;
using Northwind.Application.Common.Responses;

namespace Northwind.Application.Common.Interfaces
{
    public interface IGenericService<TEntity, TId> where TEntity : class
    {
        Task<Response<IEnumerable<TEntity>>> GetAllAsync();
        Task<PagedResponse<TEntity>> GetAllAsync(PaginationQuery paginationQuery);
        Task<Response<TEntity>> GetAsync(TId id);
        Task<Response<TEntity>> CreateAsync(TEntity obj);
        Task<Response<TEntity>> UpdateAsync(TEntity obj);
        Task<Response<TEntity>> DeleteAsync(TId id);
        Task<Response<IEnumerable<TEntity>>> DeleteRangeAsync(TId[] ids);
        Task<bool> IsExists(TId id);
        Task<bool> AreExists(TId[] id);
    }
}
