using Northwind.Application.Interfaces;

namespace Northwind.Application.Models
{
    public class QueryParameters<T> where T : IFilter, new()
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public Sorting Sorting { get; set; } = new Sorting();
        public T Filter { get; set; } = new T();
    }
}
