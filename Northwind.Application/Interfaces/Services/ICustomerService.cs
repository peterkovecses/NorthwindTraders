using Northwind.Application.Dtos;

namespace Northwind.Application.Interfaces.Services
{
    public interface ICustomerService : IGenericService<CustomerDto, string>
    {
    }
}
