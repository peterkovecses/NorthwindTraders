using Northwind.Application.Dtos;

namespace Northwind.Application.Interfaces.Services
{
    public interface IOrderService : IGenericService<OrderDto, int>
    {
    }
}
