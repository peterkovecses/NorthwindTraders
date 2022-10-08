using Northwind.Application.Interfaces;

namespace Northwind.Application.Models
{
    public class PaginationQuery : IPaginationQuery
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
