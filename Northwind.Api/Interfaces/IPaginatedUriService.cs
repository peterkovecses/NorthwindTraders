using Northwind.Application.Models;

namespace Northwind.Application.Interfaces.Services
{
    public interface IPaginatedUriService
    {
        (string? next, string? previous) GetNavigations(IPagination pagination);
    }
}