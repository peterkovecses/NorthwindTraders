using Northwind.Domain.Entities;

namespace Northwind.Application.Common.Interfaces
{
    public interface IGenericService<T> where T : class
    {
        Task<IEnumerable<T>> GetAsync();
        Task<T>? GetByIdAsync(int id);
        Task<int> CreateAsync(T dto);
        Task<T> DeleteAsync(T dto);
        Task<IEnumerable<T>> DeleteRangeAsync(IEnumerable<T> dtos);
    }
}
