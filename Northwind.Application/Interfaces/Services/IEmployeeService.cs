using Northwind.Application.Dtos;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces.Services
{
    public interface IEmployeeService : IGenericService<EmployeeDto, int, EmployeeFilter, Employee>
    {
    }
}
