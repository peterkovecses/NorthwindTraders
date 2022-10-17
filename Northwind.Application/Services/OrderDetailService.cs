using AutoMapper;
using Northwind.Application.Dtos;
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
        private readonly IPaginatedUriService _uriService;

        public OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper, IPaginatedUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public async Task<PagedResponse<OrderDetailDto>> GetAsync(
            QueryParameters<OrderDetailFilter> queryParameters, 
            CancellationToken token = default)
        {
            var result = await _unitOfWork.OrderDetails.GetAsync(queryParameters.Pagination, queryParameters.Sorting, token: token);
            queryParameters.SetPaginationIfNull(result.TotalItems);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<OrderDetailDto>>(result.Items)
                .ToPagedResponse(queryParameters.Pagination, result.TotalItems, next, previous);
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
            var orderDetailInDb = await _unitOfWork.OrderDetails.FindByIdAsync(key, token);

            _mapper.Map(orderDetailDto, orderDetailInDb);
            await _unitOfWork.CompleteAsync();

            return orderDetailDto.ToResponse();
        }

        public async Task<Response<IEnumerable<OrderDetailDto>>> DeleteAsync(OrderDetailKey[] ids, CancellationToken token = default)
        {
            var orderDetailsToRemove = await _unitOfWork.OrderDetails.FindByIdsAsync(ids, token);

            _unitOfWork.OrderDetails.Remove(orderDetailsToRemove);
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
