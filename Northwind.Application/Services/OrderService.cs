using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Exceptions;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Application.Services.PredicateBuilders;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly OrderPredicateBuilder _predicateBuilder;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, OrderPredicateBuilder predicateBuilder)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _predicateBuilder = predicateBuilder;
        }

        public async Task<PagedResponse<OrderDto>> GetAsync(QueryParameters<OrderFilter> queryParameters, CancellationToken token = default)
        {
            var predicate = _predicateBuilder.GetPredicate(queryParameters);
            var (totalOrders, orders) = await _unitOfWork.Orders.GetAsync(queryParameters.Pagination, queryParameters.Sorting, predicate, token);

            return _mapper.Map<IEnumerable<OrderDto>>(orders)
                .ToPagedResponse(queryParameters.Pagination, totalOrders);
        }

        public async Task<Response<OrderDto>> FindByIdAsync(int id, CancellationToken token = default)
        {
            var order = await _unitOfWork.Orders.FindByIdAsync(id, token);

            return _mapper.Map<OrderDto>(order).ToResponse();
        }

        public async Task<Response<OrderDto>> CreateAsync(OrderDto orderDto, CancellationToken token = default)
        {
            var order = _mapper.Map<Order>(orderDto);

            await _unitOfWork.Orders.AddAsync(order, token);
            await _unitOfWork.CompleteAsync();

            orderDto.OrderId = order.OrderId;

            return orderDto.ToResponse();
        }

        public async Task<Response<OrderDto>> UpdateAsync(OrderDto orderDto, CancellationToken token = default)
        {
            var orderInDb = 
                await _unitOfWork.Orders.FindByIdAsync(orderDto.OrderId, token) ?? throw new ItemNotFoundException<int>(orderDto.OrderId);
            _mapper.Map(orderDto, orderInDb);
            await _unitOfWork.CompleteAsync();

            return orderDto.ToResponse();
        }

        public async Task<Response<IEnumerable<OrderDto>>> DeleteAsync(int[] ids, CancellationToken token = default)
        {
            var ordersToRemove = (await _unitOfWork.Orders.GetAsync(Pagination.NoPagination(), Sorting.NoSorting(), o => ids.Contains(o.OrderId), token)).items;

            foreach (var order in ordersToRemove)
            {
                _unitOfWork.Orders.Remove(order);

            }
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(ordersToRemove).ToResponse();
        }
    }
}
