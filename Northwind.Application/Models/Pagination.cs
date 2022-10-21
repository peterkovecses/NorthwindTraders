using Northwind.Application.Interfaces;
using static Northwind.Application.Common.PaginationConstants;

namespace Northwind.Application.Models
{
    public class Pagination : IPagination
    {
        private int _pageNumber = MinPageNumber;
        private int _pageSize = MinPageSize;

        public virtual int PageNumber
        {
            get => _pageNumber;
            init => _pageNumber = value < MinPageNumber ? MinPageNumber : value;
        }
        public virtual int PageSize
        {
            get => _pageSize;
            init => _pageSize = value < MinPageSize ? MinPageSize : value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
