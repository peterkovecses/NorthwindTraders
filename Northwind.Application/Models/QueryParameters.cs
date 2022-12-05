using Northwind.Application.Interfaces;
using Northwind.Domain.Common;

namespace Northwind.Application.Models
{
    public class QueryParameters<T> where T : IFilter<EntityBase>, new()
    {
        public Pagination Pagination { get; init; } = Pagination.DefaultPagination();
        public Sorting Sorting { get; init; } = Sorting.NoSorting();
        public virtual T Filter { get; init; } = new();
    }
}
