using Northwind.Domain.Common.Models;
using Northwind.Domain.Entities;

namespace Northwind.Domain.Common.Interfaces.Repositories
{
    public interface IOrderDetailRepository : IGenericRepository<OrderDetail, OrderDetailKey>
    {
    }
}
