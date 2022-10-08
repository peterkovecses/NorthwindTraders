using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Queries;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginatedUriService _uriService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IPaginatedUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public async Task<PagedResponse<OrderDto>> GetAsync(QueryParameters queryParameters)
        {
            var (totalItems, orders) = await _unitOfWork.Orders.GetAsync(queryParameters.Pagination, queryParameters.Sorting);
            queryParameters.SetPaginationIfNull(totalItems);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<OrderDto>>(orders)
                .ToPagedResponse(queryParameters.Pagination, totalItems, next, previous);
        }

        public async Task<Response<OrderDto>> FindByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.FindByIdAsync(id);

            return _mapper.Map<OrderDto>(order).ToResponse();
        }

        public async Task<Response<OrderDto>> CreateAsync(OrderDto orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            orderDto.OrderId = order.OrderId;

            return orderDto.ToResponse();
        }

        public async Task<Response<OrderDto>> UpdateAsync(OrderDto orderDto)
        {
            var orderInDb = await _unitOfWork.Orders.FindByIdAsync(orderDto.OrderId);

            _mapper.Map(orderDto, orderInDb);
            await _unitOfWork.CompleteAsync();

            return orderDto.ToResponse();
        }

        public async Task<Response<IEnumerable<OrderDto>>> DeleteAsync(int[] ids)
        {
            var orders = (await _unitOfWork.Orders.GetAsync(predicate: o => ids.Contains(o.OrderId))).items;

            _unitOfWork.Orders.Remove(orders);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders).ToResponse();
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Orders.FindByIdAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Orders.GetAsync(predicate: o => ids.Contains(o.OrderId))).items.Count() == ids.Length;
        }
    }
}
