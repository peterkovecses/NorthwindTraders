using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Models;
using Northwind.Application.Common.Queries;
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

        public OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDetailDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var orderDetails = await _unitOfWork.OrderDetails.GetAllAsync(paginationFilter);

            return _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetails);
        }

        public async Task<OrderDetailDto>? GetAsync(OrderDetailKey id)
        {
            var orderDetail = await _unitOfWork.OrderDetails.GetAsync(ConvertToDomainObject(id));

            return _mapper.Map<OrderDetailDto>(orderDetail);
        }

        public async Task<OrderDetailKey> CreateAsync(OrderDetailDto orderDetailDto)
        {
            var orderDetail = _mapper.Map<OrderDetail>(orderDetailDto);

            await _unitOfWork.OrderDetails.AddAsync(orderDetail);
            await _unitOfWork.CompleteAsync();

            return new OrderDetailKey(orderDetail.OrderId, orderDetail.ProductId);
        }

        public async Task UpdateAsync(OrderDetailDto orderDetailDto)
        {
            var key = new OrderDetailKey(orderDetailDto.OrderId, orderDetailDto.ProductId);
            var orderDetailInDb = await _unitOfWork.OrderDetails.GetAsync(ConvertToDomainObject(key));

            _mapper.Map(orderDetailDto, orderDetailInDb);

            await _unitOfWork.CompleteAsync();
        }

        public async Task<OrderDetailDto> DeleteAsync(OrderDetailKey id)
        {
            var orderDetail = await _unitOfWork.OrderDetails.GetAsync(ConvertToDomainObject(id));

            _unitOfWork.OrderDetails.Remove(orderDetail);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<OrderDetailDto>(orderDetail);
        }

        public async Task<IEnumerable<OrderDetailDto>> DeleteRangeAsync(OrderDetailKey[] ids)
        {
            var orderDetailsToRemove = await _unitOfWork.OrderDetails.GetAsync(ConvertToDomainObject(ids));

            _unitOfWork.OrderDetails.RemoveRange(orderDetailsToRemove);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetailsToRemove);
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
