using System.Collections.Generic;

namespace Northwind.Application.Common.Queries
{
    public class PaginationQuery
    {
        private int _pageNumber;
        private int _pageSize;
        private const int MinPageNumber = 1;
        private const int MinPageSize = 1;
        private const int MaxPageSize = 100;

        public int PageNumber 
        { 
            get => _pageNumber; 
            set => _pageNumber = value < MinPageNumber ? MinPageNumber : value;
        }
        public int PageSize 
        { 
            get => _pageSize; 
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value < MinPageSize ? MinPageSize : value;
        }    
    }
}
