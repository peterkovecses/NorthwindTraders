using Northwind.Application.Interfaces;

namespace Northwind.Application.Models
{
    public class QueryParameters<T> where T : IFilter
    {
        public Pagination? Pagination { get; set; }
        public Sorting? Sorting { get; set; }
        public T? Filter { get; set; }
    }
}
