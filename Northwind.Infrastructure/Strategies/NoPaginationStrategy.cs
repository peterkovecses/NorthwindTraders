﻿using Microsoft.EntityFrameworkCore;
using Northwind.Application.Interfaces;
using Northwind.Application.Models;

namespace Northwind.Infrastructure.Strategies
{
    public class NoPaginationStrategy<TEntity> : IPaginationStrategy<TEntity> where TEntity : class
    {
        private readonly IQueryable<TEntity> _query;

        public NoPaginationStrategy(IQueryable<TEntity> query, Pagination? pagination = null)
        {
            _query = query;
        }

        public async Task<(int, IEnumerable<TEntity>)> GetItemsAsync()
        {
            var nonPaginatedItems = await _query.ToListAsync();
            return (nonPaginatedItems.Count, nonPaginatedItems);
        }
    }
}
