using Northwind.Application.Models;

namespace Northwind.Application.Interfaces.Services
{
    public interface IGenericService<TEntity, TId, TFilter> where TEntity : class where TFilter : IFilter
    {
        Task<PagedResponse<TEntity>> GetAsync(QueryParameters<TFilter> queryParameters, CancellationToken token = default);
        Task<Response<TEntity>> FindByIdAsync(TId id, CancellationToken token = default);
        Task<Response<TEntity>> CreateAsync(TEntity obj, CancellationToken token = default );
        Task<Response<TEntity>> UpdateAsync(TEntity obj, CancellationToken token = default);
        Task<Response<IEnumerable<TEntity>>> DeleteAsync(TId[] ids, CancellationToken token = default);
        Task<bool> IsExists(TId id, CancellationToken token = default);
        Task<bool> AreExists(TId[] id, CancellationToken token = default);
    }
}
