using Microsoft.AspNetCore.Mvc.ModelBinding;
using Northwind.Application.Exceptions;
using Northwind.Application.Interfaces;

namespace Northwind.Application.Models
{
    public class Pagination : IPagination
    {
        public const int MinPageNumber = 1;
        public const int MinPageSize = 1;
        public const int NoPaginationPageNumber = 0;
        public const int NoPaginationPageSize = int.MaxValue;
        public const int MaxPageSize = 5000;

        private int _pageNumber;
        private int _pageSize;

        public Pagination()
        {
            _pageNumber = MinPageNumber;
            _pageSize = MaxPageSize;
        }

        public virtual int PageNumber
        {
            get => _pageNumber;
            init => _pageNumber = value < MinPageNumber ? throw new ArgumentOutOfRangeException(nameof(PageNumber)) : value;
        }

        public virtual int PageSize
        {
            get => _pageSize;
            init => _pageSize = value < MinPageSize ? throw new ArgumentOutOfRangeException(nameof(PageSize)) : value > MaxPageSize ? throw new ValueAboveMaxPageSizeException(value) : value;
        }

        [BindNever]
        public bool IsNoPagination => _pageNumber == NoPaginationPageNumber;

        public static Pagination NoPagination() => new() { _pageNumber = NoPaginationPageNumber, _pageSize = NoPaginationPageSize };
        public static Pagination DefaultPagination() => new() { _pageNumber = MinPageNumber, _pageSize = MaxPageSize };

    }
}
