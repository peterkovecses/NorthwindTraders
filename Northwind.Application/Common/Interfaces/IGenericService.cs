namespace Northwind.Application.Common.Interfaces
{
    public interface IGenericService<T> where T : class
    {
        Task<IEnumerable<T>> GetAsync();
        Task<T>? GetByIdAsync(int id);
        Task<int> CreateAsync(T dto);
        Task UpdateAsync(T dto);
        Task<T> DeleteAsync(int id);
        Task<IEnumerable<T>> DeleteRangeAsync(int[] ids);
        Task<bool> IsExists(int id);
        Task<bool> AreExists(int[] id);
    }
}
