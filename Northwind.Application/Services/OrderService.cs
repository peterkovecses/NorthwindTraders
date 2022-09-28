using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Dtos;
using Northwind.Domain.Common.Interfaces;
using Northwind.Domain.Common.Queries;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var orders = await _unitOfWork.Orders.GetAllAsync(paginationFilter);

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto>? GetAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetAsync(id);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<int> CreateAsync(OrderDto orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            return order.OrderId;
        }

        public async Task UpdateAsync(OrderDto orderDto)
        {
            var orderInDb = await _unitOfWork.Orders.GetAsync(orderDto.OrderId);

            _mapper.Map(orderDto, orderInDb);

            await _unitOfWork.CompleteAsync();
        }

        public async Task<OrderDto> DeleteAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetAsync(id);

            _unitOfWork.Orders.Remove(order);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> DeleteRangeAsync(int[] ids)
        {
            var orders = await _unitOfWork.Orders.FindAllAsync(o => ids.Contains(o.OrderId));

            _unitOfWork.Orders.RemoveRange(orders);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Orders.GetAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Orders.FindAllAsync(o => ids.Contains(o.OrderId))).Count() == ids.Length;
        }
    }
}
