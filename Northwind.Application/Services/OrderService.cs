﻿using AutoMapper;
using Northwind.Application.Common.Extensions;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Common.Responses;
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
        private readonly IPaginatedUriService _uriService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IPaginatedUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public async Task<Response<IEnumerable<OrderDto>>> GetAllAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders).ToResponse();
        }

        public async Task<PagedResponse<OrderDto>> GetAllAsync(PaginationQuery paginationQuery)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var (totalItems, orders) = await _unitOfWork.Orders.GetAllAsync(paginationFilter);
            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return _mapper.Map<IEnumerable<OrderDto>>(orders)
                .ToPagedResponse(paginationQuery, totalItems, next, previous);
        }

        public async Task<Response<OrderDto>> GetAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetAsync(id);

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
            var orderInDb = await _unitOfWork.Orders.GetAsync(orderDto.OrderId);

            _mapper.Map(orderDto, orderInDb);
            await _unitOfWork.CompleteAsync();

            return orderDto.ToResponse();
        }

        public async Task<Response<OrderDto>> DeleteAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetAsync(id);

            _unitOfWork.Orders.Remove(order);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<OrderDto>(order).ToResponse();
        }

        public async Task<Response<IEnumerable<OrderDto>>> DeleteRangeAsync(int[] ids)
        {
            var orders = await _unitOfWork.Orders.FindAllAsync(o => ids.Contains(o.OrderId));

            _unitOfWork.Orders.RemoveRange(orders);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders).ToResponse();
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