using Northwind.Application.Dtos;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces.Services
{
    public interface ISupplierService : IGenericService<SupplierDto, int, SupplierFilter, Supplier>
    {
    }
}
