using Northwind.Application.Dtos;
using Northwind.Domain.Common.Models;

namespace Northwind.Application.Common.Interfaces
{
    public interface IOrderDetailService : IGenericService<OrderDetailDto, OrderDetailKey>
    {
    }
}
