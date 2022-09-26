using Northwind.Application.Dtos;
using Northwind.Domain.Entities;

namespace Northwind.Application.Common.Interfaces
{
    public interface IEmployeeService : IGenericService<EmployeeDto, int>
    {
    }
}
