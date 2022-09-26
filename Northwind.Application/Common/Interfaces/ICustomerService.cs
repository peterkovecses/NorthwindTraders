using Northwind.Application.Dtos;

namespace Northwind.Application.Common.Interfaces
{
    public interface ICustomerService : IGenericService<CustomerDto, string>
    {
    }
}
