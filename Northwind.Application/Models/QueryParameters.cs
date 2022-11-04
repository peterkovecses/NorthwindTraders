using Northwind.Application.Interfaces;

namespace Northwind.Application.Models
{
    public class QueryParameters<T> where T : IFilter, new()
    {
        public Pagination Pagination { get; init; } = Pagination.DefaultPagination();
        public Sorting Sorting { get; init; } = Sorting.NoSorting();
        public T Filter { get; init; } = new T();
    }
}
