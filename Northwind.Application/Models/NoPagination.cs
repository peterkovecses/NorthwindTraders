using Northwind.Application.Interfaces;
using static Northwind.Application.Common.PaginationConstants;

namespace Northwind.Application.Models
{
    public class NoPagination : Pagination
    {
        public override int PageNumber { get; init; } = MinPageNumber;
        public override int PageSize { get; init;  } = MaxPageSize;
    }
}
