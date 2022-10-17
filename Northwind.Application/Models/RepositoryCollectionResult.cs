namespace Northwind.Application.Models
{
    public record RepositoryCollectionResult<TEntity> (int TotalItems, IEnumerable<TEntity> Items) where TEntity : class;
}
