using Northwind.Application.Interfaces;

namespace Northwind.Application.Models
{
    public class Pagination : IPagination
    {
        public const int MinPageNumber = 1;
        public const int MinPageSize = 1;
        public const int MaxPageSize = 5000;

        private int _pageNumber = MinPageNumber;
        private int _pageSize = MaxPageSize;

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

        public bool IsNoPagination { get; private init; }

        public static Pagination NoPagination => new() { _pageNumber = default, _pageSize = default, IsNoPagination = true };
    }
}
