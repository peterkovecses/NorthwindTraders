namespace Northwind.Application.Models
{
    public class Pagination
    {
        private const int MinPageNumber = 1;
        private const int MinPageSize = 1;
        private int _pageNumber = MinPageNumber;
        private int _pageSize = MinPageSize;

        public int PageNumber 
        { 
            get => _pageNumber; 
            set => _pageNumber = value < MinPageNumber ? MinPageNumber : value; 
        }
        public int PageSize 
        { 
            get => _pageSize; 
            set => _pageSize = value < MinPageSize ? MinPageSize : value; 
        }
    }
}
