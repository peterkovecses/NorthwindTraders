using Northwind.Application.Common.Queries;

namespace Northwind.Application.Common.Interfaces
{
    public interface IGenericService<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(PaginationQuery? paginationQuery = null);
        Task<T>? GetAsync(int id);
        Task<int> CreateAsync(T obj);
        Task UpdateAsync(T obj);
        Task<T> DeleteAsync(int id);
        Task<IEnumerable<T>> DeleteRangeAsync(int[] ids);
        Task<bool> IsExists(int id);
        Task<bool> AreExists(int[] id);
    }
}
