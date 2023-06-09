﻿using Microsoft.EntityFrameworkCore;
using Northwind.Application.Exceptions;
using Northwind.Application.Interfaces;

namespace Northwind.Application.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task<IEnumerable<TEntity>> Paginate<TEntity>(this IQueryable<TEntity> query, IPagination pagination, int totalItems, CancellationToken token) where TEntity : class
        {
            if (totalItems > 0)
            {
                if (pagination.IsNoPagination)
                {
                    return await query.ToListAsync(token);
                }

                var items = await query
                .Skip(pagination.GetItemsToSkip())
                .Take(pagination.GetItemsToTake(totalItems))
                .ToListAsync(token);

                if (pagination.PageNumber > 1 && !items.Any())
                {
                    throw new PaginationException(pagination.PageNumber);
                }

                return items;
            }
            else
            {
                return new List<TEntity>();
            }
        }

        private static int GetItemsToSkip(this IPagination pagination)
        {
            return (pagination.PageNumber - 1) * pagination.PageSize;
        }

        private static int GetItemsToTake(this IPagination pagination, int totalItems)
        {
            if (pagination.PageSize == 0)
            {
                return totalItems;
            }

            return pagination.PageSize;
        }
    }
}
