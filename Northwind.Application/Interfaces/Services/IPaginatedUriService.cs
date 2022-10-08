namespace Northwind.Application.Interfaces.Services
{
    public interface IPaginatedUriService
    {
        (string? next, string? previous) GetNavigations(IPaginationQuery paginationQuery);
    }
}