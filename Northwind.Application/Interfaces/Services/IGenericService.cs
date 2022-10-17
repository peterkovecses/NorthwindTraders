using Northwind.Application.Models;

namespace Northwind.Application.Interfaces.Services
{
    public interface IGenericService<TEntity, TId, TFilter> where TEntity : class where TFilter : IFilter
    {
        Task<PagedResponse<TEntity>> GetAsync(QueryParameters<TFilter> queryParameters);
        Task<Response<TEntity>> FindByIdAsync(TId id);
        Task<Response<TEntity>> CreateAsync(TEntity obj);
        Task<Response<TEntity>> UpdateAsync(TEntity obj);
        Task<Response<IEnumerable<TEntity>>> DeleteAsync(TId[] ids);
        Task<bool> IsExists(TId id);
        Task<bool> AreExists(TId[] id);
    }
}
