using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
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

        public async Task<PagedResponse<ShipperDto>> GetAsync(IPaginationQuery? paginationQuery = null)
        {
            var (totalItems, shippers) = await _unitOfWork.Shippers.GetAsync(paginationQuery);
            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return _mapper.Map<IEnumerable<ShipperDto>>(shippers)
                .ToPagedResponse(paginationQuery, totalItems, next, previous);
        }

        public async Task<Response<ShipperDto>> FindByIdAsync(int id)
        {
            var shipper = await _unitOfWork.Shippers.FindByIdAsync(id);

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
            var shipperInDb = await _unitOfWork.Regions.FindByIdAsync(shipperDto.ShipperId);

            _mapper.Map(shipperDto, shipperInDb);
            await _unitOfWork.CompleteAsync();

            return shipperDto.ToResponse();
        }

        public async Task<Response<IEnumerable<ShipperDto>>> DeleteAsync(int[] ids)
        {
            var shippers = (await _unitOfWork.Shippers.GetAsync(predicate: s => ids.Contains(s.ShipperId))).items;

            _unitOfWork.Shippers.Remove(shippers);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<ShipperDto>>(shippers).ToResponse();
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Shippers.FindByIdAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Shippers.GetAsync(predicate: s => ids.Contains(s.ShipperId))).items.Count() == ids.Length;
        }
    }
}
