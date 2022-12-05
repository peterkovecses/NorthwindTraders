using Northwind.Application.Interfaces;
using Northwind.Domain.Common;

namespace Northwind.Application.Models
{
    public class QueryParameters<TFilter, TFilterEntity> 
        where TFilter : IFilter<TFilterEntity>, new()
        where TFilterEntity : EntityBase
    {
        public Pagination Pagination { get; init; } = Pagination.DefaultPagination();
        public Sorting Sorting { get; init; } = Sorting.NoSorting();
        public virtual TFilter Filter { get; init; } = new();
    }
}
