using Northwind.Application.Models;

namespace Northwind.Application.Exceptions
{
    public class ValueAboveMaxPageSizeException : Exception
    {
        public int PageSize { get; }

        public ValueAboveMaxPageSizeException(int pageSize) : base($"Page size {pageSize} is above the maximum page size ({Pagination.MaxPageSize}).")
        {
            PageSize = pageSize;
        }

    }
}
