using Northwind.Application.Models;
using Northwind.Domain.Common;

namespace Northwind.Application.Interfaces.Services
{
    public interface IGenericService<TEntity, TId, TFilter, TFilterEntity>
        where TEntity : class
        where TFilter : IFilter<TFilterEntity>, new()
        where TFilterEntity : EntityBase
    {
        Task<PagedResponse<TEntity>> GetAsync(QueryParameters<TFilter, TFilterEntity> queryParameters, CancellationToken token);
        Task<Response<TEntity>> FindByIdAsync(TId id, CancellationToken token);
        Task<Response<TEntity>> CreateAsync(TEntity obj, CancellationToken token);
        Task<Response<TEntity>> UpdateAsync(TEntity obj, CancellationToken token);
        Task DeleteAsync(TId id, CancellationToken token);
    }
}
