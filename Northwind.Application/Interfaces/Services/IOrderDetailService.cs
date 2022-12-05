using Northwind.Application.Dtos;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces.Services
{
    public interface IOrderDetailService : IGenericService<OrderDetailDto, OrderDetailKey, OrderDetailFilter, OrderDetail>
    {
    }
}
