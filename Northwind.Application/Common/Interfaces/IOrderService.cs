using Northwind.Application.Dtos;

namespace Northwind.Application.Common.Interfaces
{
    public interface IOrderService : IGenericService<OrderDto, int>
    {
    }
}
