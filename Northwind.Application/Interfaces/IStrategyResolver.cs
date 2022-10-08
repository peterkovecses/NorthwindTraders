using Northwind.Application.Models;

namespace Northwind.Application.Interfaces;

public interface IStrategyResolver
{
    IPaginationStrategy<TEntity> GetStrategy<TEntity>(IQueryable<TEntity> query, Pagination? paginationQuery) where TEntity : class;
}