using Northwind.Application.Models;
using Northwind.Domain.Common;

namespace Northwind.Application.Interfaces.Services
{
    public interface IGenericService<TEntity, TId, TFilter>
        where TEntity : class
        where TFilter : IFilter<EntityBase>, new() 
    {
        Task<PagedResponse<TEntity>> GetAsync(QueryParameters<TFilter> queryParameters, CancellationToken token);
        Task<Response<TEntity>> FindByIdAsync(TId id, CancellationToken token);
        Task<Response<TEntity>> CreateAsync(TEntity obj, CancellationToken token);
        Task<Response<TEntity>> UpdateAsync(TEntity obj, CancellationToken token);
        Task<Response<IEnumerable<TEntity>>> DeleteAsync(TId[] ids, CancellationToken token);
    }
}
