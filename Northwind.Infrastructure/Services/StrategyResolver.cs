﻿using Northwind.Application.Interfaces;
using Northwind.Application.Models;
using Northwind.Infrastructure.Strategies;

namespace Northwind.Infrastructure.Services
{
    public class StrategyResolver : IStrategyResolver
    {
        public IPaginationStrategy<TEntity> GetStrategy<TEntity>(IQueryable<TEntity> query, Pagination? paginationQuery) where TEntity : class
        {
            if (paginationQuery == null)
            {
                return new NoPaginationStrategy<TEntity>(query);
            }

            return new PaginationStrategy<TEntity>(query, paginationQuery);
        }
    }
}
