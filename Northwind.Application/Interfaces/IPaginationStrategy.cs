namespace Northwind.Application.Interfaces
{
    public interface IPaginationStrategy<TEntity> where TEntity : class
    {
        Task<(int totalItems, IEnumerable<TEntity> items)> GetItemsAsync(CancellationToken token);
    }
}
