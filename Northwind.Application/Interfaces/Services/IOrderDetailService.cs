using Northwind.Application.Dtos;
using Northwind.Application.Models;

namespace Northwind.Application.Interfaces.Services
{
    public interface IOrderDetailService : IGenericService<OrderDetailDto, OrderDetailKey>
    {
    }
}
