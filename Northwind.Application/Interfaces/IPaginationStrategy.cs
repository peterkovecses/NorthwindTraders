namespace Northwind.Application.Interfaces
{
    public interface IPaginationStrategy<TEntity> where TEntity : class
    {
        Task<(int, IEnumerable<TEntity>)> GetItemsAsync();
    }
}
