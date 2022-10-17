using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces.Repositories
{
    public interface IOrderDetailRepository : IGenericRepository<OrderDetail, IOrderDetailKey>
    {
        Task<IEnumerable<OrderDetail>>? FindByIdsAsync(IOrderDetailKey[] keys, CancellationToken token);
    }
}
