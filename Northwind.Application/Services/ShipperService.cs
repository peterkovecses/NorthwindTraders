﻿using AutoMapper;
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
    public class ShipperService : IShipperService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShipperService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ShipperDto>> GetAsync(QueryParameters<ShipperFilter, Shipper> queryParameters, CancellationToken token = default)
        {
            var (totalShippers, shippers) = await _unitOfWork.Shippers.GetAsync(queryParameters.Pagination, queryParameters.Sorting, queryParameters.Filter.GetPredicate(), token);

            return _mapper.Map<IEnumerable<ShipperDto>>(shippers)
                .ToPagedResponse(queryParameters.Pagination, totalShippers);
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
            var shipperInDb = 
                await _unitOfWork.Regions.FindByIdAsync(shipperDto.ShipperId, token) ?? throw new ItemNotFoundException<int>(shipperDto.ShipperId);
            _mapper.Map(shipperDto, shipperInDb);
            await _unitOfWork.CompleteAsync();

            return shipperDto.ToResponse();
        }

        public async Task DeleteAsync(int id, CancellationToken token = default)
        {
            var shipperToRemove = await _unitOfWork.Shippers.FindByIdAsync(id, token);
            _unitOfWork.Shippers.Remove(shipperToRemove);

            await _unitOfWork.CompleteAsync();
        }
    }
}
