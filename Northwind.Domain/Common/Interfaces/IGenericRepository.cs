namespace Northwind.Domain.Common.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAsync();
        Task<TEntity>? GetByIdAsync(int id);
        Task AddAsync(TEntity entity);           
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
