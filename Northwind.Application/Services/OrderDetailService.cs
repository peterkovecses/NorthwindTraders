using AutoMapper;
using Northwind.Application.Common.Extensions;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Models;
using Northwind.Application.Common.Queries;
using Northwind.Application.Common.Responses;
using Northwind.Application.Dtos;
using Northwind.Domain.Common.Interfaces;
using Northwind.Domain.Common.Queries;
using Northwind.Domain.Entities;
using DomainModels = Northwind.Domain.Common.Models;

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

        public async Task<Response<IEnumerable<OrderDetailDto>>> GetAllAsync()
        {
            var orderDetails = await _unitOfWork.OrderDetails.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetails).ToResponse();
        }

        public async Task<PagedResponse<OrderDetailDto>> GetAllAsync(PaginationQuery paginationQuery)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var (totalItems, orderDetails) = await _unitOfWork.OrderDetails.GetAllAsync(paginationFilter);
            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetails)
                .ToPagedResponse()
                .SetPagination(paginationQuery, next, previous, totalItems);
        }

        public async Task<Response<OrderDetailDto>> GetAsync(OrderDetailKey id)
        {
            var orderDetail = await _unitOfWork.OrderDetails.GetAsync(ConvertToDomainObject(id));

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
            var orderDetailInDb = await _unitOfWork.OrderDetails.GetAsync(ConvertToDomainObject(key));

            _mapper.Map(orderDetailDto, orderDetailInDb);
            await _unitOfWork.CompleteAsync();

            return orderDetailDto.ToResponse();
        }

        public async Task<Response<OrderDetailDto>> DeleteAsync(OrderDetailKey id)
        {
            var orderDetail = await _unitOfWork.OrderDetails.GetAsync(ConvertToDomainObject(id));

            _unitOfWork.OrderDetails.Remove(orderDetail);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<OrderDetailDto>(orderDetail).ToResponse();
        }

        public async Task<Response<IEnumerable<OrderDetailDto>>> DeleteRangeAsync(OrderDetailKey[] ids)
        {
            var orderDetailsToRemove = await _unitOfWork.OrderDetails.GetAsync(ConvertToDomainObject(ids));

            _unitOfWork.OrderDetails.RemoveRange(orderDetailsToRemove);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetailsToRemove).ToResponse();
        }

        public async Task<bool> IsExists(OrderDetailKey id)
        {
            return await _unitOfWork.OrderDetails.GetAsync(ConvertToDomainObject(id)) != null;
        }

        public async Task<bool> AreExists(OrderDetailKey[] ids)
        {
            var orderDetails = await _unitOfWork.OrderDetails.GetAsync(ConvertToDomainObject(ids));

            return orderDetails.Count() == ids.Length;
        }

        private DomainModels.OrderDetailKey ConvertToDomainObject(OrderDetailKey key)
        {
            return _mapper.Map<DomainModels.OrderDetailKey>(key);
        }

        private DomainModels.OrderDetailKey[] ConvertToDomainObject(OrderDetailKey[] keys)
        {
            return _mapper.Map<DomainModels.OrderDetailKey[]>(keys);
        }
    }
}
