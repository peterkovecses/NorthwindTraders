using Northwind.Application.Dtos;

namespace Northwind.Application.Interfaces.Services
{
    public interface ICustomerDemographicService : IGenericService<CustomerDemographicDto, string>
    {
    }
}