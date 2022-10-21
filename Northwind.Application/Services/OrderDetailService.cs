using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Exceptions;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<OrderDetailDto>> GetAsync(
            QueryParameters<OrderDetailFilter> queryParameters, 
            CancellationToken token = default)
        {
            var (totalOrderDetails, orderDetails) = await _unitOfWork.OrderDetails.GetAsync(queryParameters.Pagination, queryParameters.Sorting, token: token);
            queryParameters.SetPaginationIfNull(totalOrderDetails);

            return _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetails)
                .ToPagedResponse(queryParameters.Pagination, totalOrderDetails);
        }

        public async Task<Response<OrderDetailDto>> FindByIdAsync(OrderDetailKey id, CancellationToken token = default)
        {
            var orderDetail = await _unitOfWork.OrderDetails.FindByIdAsync(id, token);

            return _mapper.Map<OrderDetailDto>(orderDetail).ToResponse();
        }

        public async Task<Response<OrderDetailDto>> CreateAsync(OrderDetailDto orderDetailDto, CancellationToken token = default)
        {
            var orderDetail = _mapper.Map<OrderDetail>(orderDetailDto);

            await _unitOfWork.OrderDetails.AddAsync(orderDetail, token);
            await _unitOfWork.CompleteAsync();

            orderDetailDto.OrderId = orderDetail.OrderId;
            orderDetailDto.ProductId = orderDetail.ProductId;

            return orderDetailDto.ToResponse();
        }

        public async Task<Response<OrderDetailDto>> UpdateAsync(OrderDetailDto orderDetailDto, CancellationToken token = default)
        {
            var key = new OrderDetailKey(orderDetailDto.OrderId, orderDetailDto.ProductId);
            var orderDetailInDb = 
                await _unitOfWork.OrderDetails.FindByIdAsync(key, token) ?? throw new ItemNotFoundException((key.OrderId, key.ProductId));
            _mapper.Map(orderDetailDto, orderDetailInDb);
            await _unitOfWork.CompleteAsync();

            return orderDetailDto.ToResponse();
        }

        public async Task<Response<IEnumerable<OrderDetailDto>>> DeleteAsync(OrderDetailKey[] ids, CancellationToken token = default)
        {
            var orderDetailsToRemove = await _unitOfWork.OrderDetails.FindByIdsAsync(ids, token);

            foreach (var orderDetail in orderDetailsToRemove)
            {
                _unitOfWork.OrderDetails.Remove(orderDetail);
            }
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetailsToRemove).ToResponse();
        }

        public async Task<bool> IsExists(OrderDetailKey id, CancellationToken token = default)
        {
            return await _unitOfWork.OrderDetails.FindByIdAsync(id, token) != null;
        }

        public async Task<bool> AreExists(OrderDetailKey[] ids, CancellationToken token = default)
        {
            var orderDetails = await _unitOfWork.OrderDetails.FindByIdsAsync(ids, token);

            return orderDetails.Count() == ids.Length;
        }
    }
}
