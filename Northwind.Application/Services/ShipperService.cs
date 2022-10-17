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

        public async Task<PagedResponse<ShipperDto>> GetAsync(QueryParameters<ShipperFilter> queryParameters, CancellationToken token = default)
        {
            var result = await _unitOfWork.Shippers.GetAsync(queryParameters.Pagination, queryParameters.Sorting, token: token);
            queryParameters.SetPaginationIfNull(result.TotalItems);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<ShipperDto>>(result.Items)
                .ToPagedResponse(queryParameters.Pagination, result.TotalItems, next, previous);
        }

        public async Task<Response<ShipperDto>> FindByIdAsync(int id, CancellationToken token = default)
        {
            var shipper = await _unitOfWork.Shippers.FindByIdAsync(id, token);

            return _mapper.Map<ShipperDto>(shipper).ToResponse();
        }

        public async Task<Response<ShipperDto>> CreateAsync(ShipperDto shipperDto, CancellationToken token = default)
        {
            var shipper = _mapper.Map<Shipper>(shipperDto);

            await _unitOfWork.Shippers.AddAsync(shipper, token);
            await _unitOfWork.CompleteAsync();

            shipperDto.ShipperId = shipper.ShipperId;

            return shipperDto.ToResponse();
        }

        public async Task<Response<ShipperDto>> UpdateAsync(ShipperDto shipperDto, CancellationToken token = default)
        {
            var shipperInDb = await _unitOfWork.Regions.FindByIdAsync(shipperDto.ShipperId, token);

            _mapper.Map(shipperDto, shipperInDb);
            await _unitOfWork.CompleteAsync();

            return shipperDto.ToResponse();
        }

        public async Task<Response<IEnumerable<ShipperDto>>> DeleteAsync(int[] ids, CancellationToken token = default)
        {
            var shippers = (await _unitOfWork.Shippers.GetAsync(predicate: s => ids.Contains(s.ShipperId), token: token)).Items;

            _unitOfWork.Shippers.Remove(shippers);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<ShipperDto>>(shippers).ToResponse();
        }

        public async Task<bool> IsExists(int id, CancellationToken token = default)
        {
            return await _unitOfWork.Shippers.FindByIdAsync(id, token) != null;
        }

        public async Task<bool> AreExists(int[] ids, CancellationToken token = default)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Shippers.GetAsync(predicate: s => ids.Contains(s.ShipperId), token: token)).Items.Count() == ids.Length;
        }
    }
}
