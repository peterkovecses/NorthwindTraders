using Northwind.Application.Common.Queries;

namespace Northwind.Application.Common.Interfaces
{
    public interface IPaginatedUriService
    {
        (string? next, string? previous) GetNavigations(PaginationQuery paginationQuery);
    }
}