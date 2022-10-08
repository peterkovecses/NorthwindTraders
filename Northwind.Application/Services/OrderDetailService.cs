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
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginatedUriService _uriService;

        public OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper, IPaginatedUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public async Task<PagedResponse<OrderDetailDto>> GetAsync(QueryParameters queryParameters)
        {
            var (totalItems, orderDetails) = await _unitOfWork.OrderDetails.GetAsync(queryParameters.Pagination, queryParameters.Sorting);
            queryParameters.SetPaginationIfNull(totalItems);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetails)
                .ToPagedResponse(queryParameters.Pagination, totalItems, next, previous);
        }

        public async Task<Response<OrderDetailDto>> FindByIdAsync(OrderDetailKey id)
        {
            var orderDetail = await _unitOfWork.OrderDetails.FindByIdAsync(id);

            return _mapper.Map<OrderDetailDto>(orderDetail).ToResponse();
        }

        public async Task<Response<OrderDetailDto>> CreateAsync(OrderDetailDto orderDetailDto)
        {
            var orderDetail = _mapper.Map<OrderDetail>(orderDetailDto);

            await _unitOfWork.OrderDetails.AddAsync(orderDetail);
            await _unitOfWork.CompleteAsync();

            orderDetailDto.OrderId = orderDetail.OrderId;
            orderDetailDto.ProductId = orderDetail.ProductId;

            return orderDetailDto.ToResponse();
        }

        public async Task<Response<OrderDetailDto>> UpdateAsync(OrderDetailDto orderDetailDto)
        {
            var key = new OrderDetailKey(orderDetailDto.OrderId, orderDetailDto.ProductId);
            var orderDetailInDb = await _unitOfWork.OrderDetails.FindByIdAsync(key);

            _mapper.Map(orderDetailDto, orderDetailInDb);
            await _unitOfWork.CompleteAsync();

            return orderDetailDto.ToResponse();
        }

        public async Task<Response<IEnumerable<OrderDetailDto>>> DeleteAsync(OrderDetailKey[] ids)
        {
            var orderDetailsToRemove = await _unitOfWork.OrderDetails.FindByIdsAsync(ids);

            _unitOfWork.OrderDetails.Remove(orderDetailsToRemove);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetailsToRemove).ToResponse();
        }

        public async Task<bool> IsExists(OrderDetailKey id)
        {
            return await _unitOfWork.OrderDetails.FindByIdAsync(id) != null;
        }

        public async Task<bool> AreExists(OrderDetailKey[] ids)
        {
            var orderDetails = await _unitOfWork.OrderDetails.FindByIdsAsync(ids);

            return orderDetails.Count() == ids.Length;
        }
    }
}
