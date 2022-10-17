using Northwind.Application.Dtos;
using Northwind.Application.Models.Filters;

namespace Northwind.Application.Interfaces.Services
{
    public interface ICustomerDemographicService : IGenericService<CustomerDemographicDto, string, CustomerDemographicFilter>
    {
    }
}