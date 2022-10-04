using AutoMapper;
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
    public class ShipperService : IShipperService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginatedUriService _uriService;

        public ShipperService(IUnitOfWork unitOfWork, IMapper mapper, IPaginatedUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public async Task<Response<IEnumerable<ShipperDto>>> GetAllAsync()
        {
            var shippers = await _unitOfWork.Shippers.GetAllAsync();
            return _mapper.Map<IEnumerable<ShipperDto>>(shippers).ToResponse();
        }

        public async Task<PagedResponse<ShipperDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var (totalItems, shippers) = await _unitOfWork.Shippers.GetAllAsync(paginationFilter);
            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return _mapper.Map<IEnumerable<ShipperDto>>(shippers)
                .ToPagedResponse(paginationQuery, totalItems, next, previous);
        }

        public async Task<Response<ShipperDto>> GetAsync(int id)
        {
            var shipper = await _unitOfWork.Shippers.GetAsync(id);

            return _mapper.Map<ShipperDto>(shipper).ToResponse();
        }

        public async Task<Response<ShipperDto>> CreateAsync(ShipperDto shipperDto)
        {
            var shipper = _mapper.Map<Shipper>(shipperDto);

            await _unitOfWork.Shippers.AddAsync(shipper);
            await _unitOfWork.CompleteAsync();

            shipperDto.ShipperId = shipper.ShipperId;

            return shipperDto.ToResponse();
        }

        public async Task<Response<ShipperDto>> UpdateAsync(ShipperDto shipperDto)
        {
            var shipperInDb = await _unitOfWork.Regions.GetAsync(shipperDto.ShipperId);

            _mapper.Map(shipperDto, shipperInDb);
            await _unitOfWork.CompleteAsync();

            return shipperDto.ToResponse();
        }

        public async Task<Response<ShipperDto>> DeleteAsync(int id)
        {
            var shipper = await _unitOfWork.Shippers.GetAsync(id);

            _unitOfWork.Shippers.Remove(shipper);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ShipperDto>(shipper).ToResponse();
        }

        public async Task<Response<IEnumerable<ShipperDto>>> DeleteRangeAsync(int[] ids)
        {
            var shippers = await _unitOfWork.Shippers.FindAllAsync(s => ids.Contains(s.ShipperId));

            _unitOfWork.Shippers.RemoveRange(shippers);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<ShipperDto>>(shippers).ToResponse();
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Shippers.GetAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Shippers.FindAllAsync(s => ids.Contains(s.ShipperId))).Count() == ids.Length;
        }
    }
}
