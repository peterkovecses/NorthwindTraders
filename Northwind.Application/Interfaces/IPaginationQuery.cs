namespace Northwind.Application.Interfaces
{
    public interface IPaginationQuery
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }
    }
}