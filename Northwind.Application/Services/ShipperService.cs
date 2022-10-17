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

        public async Task<PagedResponse<ShipperDto>> GetAsync(QueryParameters<ShipperFilter> queryParameters)
        {
            var result = await _unitOfWork.Shippers.GetAsync(queryParameters.Pagination, queryParameters.Sorting);
            queryParameters.SetPaginationIfNull(result.TotalItems);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<ShipperDto>>(result.Items)
                .ToPagedResponse(queryParameters.Pagination, result.TotalItems, next, previous);
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
            var shippers = (await _unitOfWork.Shippers.GetAsync(predicate: s => ids.Contains(s.ShipperId))).Items;

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
            return (await _unitOfWork.Shippers.GetAsync(predicate: s => ids.Contains(s.ShipperId))).Items.Count() == ids.Length;
        }
    }
}
