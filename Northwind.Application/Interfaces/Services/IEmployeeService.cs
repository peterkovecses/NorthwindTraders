using Northwind.Application.Dtos;

namespace Northwind.Application.Interfaces.Services
{
    public interface IEmployeeService : IGenericService<EmployeeDto, int>
    {
    }
}
