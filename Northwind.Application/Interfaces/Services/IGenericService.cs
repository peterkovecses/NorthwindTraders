using Northwind.Application.Models;
using Northwind.Application.Models.Queries;

namespace Northwind.Application.Interfaces.Services
{
    public interface IGenericService<TEntity, TId> where TEntity : class
    {
        Task<PagedResponse<TEntity>> GetAsync(QueryParameters queryParameters);
        Task<Response<TEntity>> FindByIdAsync(TId id);
        Task<Response<TEntity>> CreateAsync(TEntity obj);
        Task<Response<TEntity>> UpdateAsync(TEntity obj);
        Task<Response<IEnumerable<TEntity>>> DeleteAsync(TId[] ids);
        Task<bool> IsExists(TId id);
        Task<bool> AreExists(TId[] id);
    }
}
