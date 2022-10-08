namespace Northwind.Application.Interfaces;

public interface IStrategyResolver
{
    IPaginationStrategy<TEntity> GetStrategy<TEntity>(IQueryable<TEntity> query, IPaginationQuery? paginationQuery) where TEntity : class;
}