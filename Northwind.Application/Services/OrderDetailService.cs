using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Dtos;
using Northwind.Domain.Common.Interfaces;
using Northwind.Domain.Common.Models;
using Northwind.Domain.Common.Queries;
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

        public async Task<IEnumerable<OrderDetailDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var orderDetails = await _unitOfWork.OrderDetails.GetAllAsync(paginationFilter);

            return _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetails);
        }

        public async Task<OrderDetailDto>? GetAsync(OrderDetailKey id)
        {
            var orderDetail = await _unitOfWork.OrderDetails.GetAsync(id);

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
            var orderDetailInDb = await _unitOfWork.OrderDetails.GetAsync(key);

            _mapper.Map(orderDetailDto, orderDetailInDb);

            await _unitOfWork.CompleteAsync();
        }

        public async Task<OrderDetailDto> DeleteAsync(OrderDetailKey id)
        {
            var orderDetail = await _unitOfWork.OrderDetails.GetAsync(id);

            _unitOfWork.OrderDetails.Remove(orderDetail);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<OrderDetailDto>(orderDetail);
        }

        public async Task<IEnumerable<OrderDetailDto>> DeleteRangeAsync(OrderDetailKey[] ids)
        {
            var orderDetails = await _unitOfWork.OrderDetails.FindAllAsync(orderDetail => ids.Contains(new OrderDetailKey(orderDetail.OrderId, orderDetail.ProductId)));

            _unitOfWork.OrderDetails.RemoveRange(orderDetails);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetails);
        }

        public async Task<bool> IsExists(OrderDetailKey id)
        {
            return await _unitOfWork.OrderDetails.GetAsync(id) != null;
        }

        public async Task<bool> AreExists(OrderDetailKey[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.OrderDetails.FindAllAsync(orderDetail => ids.Contains(new OrderDetailKey(orderDetail.OrderId, orderDetail.ProductId)))).Count() == ids.Length;
        }
    }
}
