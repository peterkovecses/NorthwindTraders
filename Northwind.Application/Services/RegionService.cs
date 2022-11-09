using AutoMapper;
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
    public class RegionService : IRegionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RegionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<RegionDto>> GetAsync(QueryParameters<RegionFilter> queryParameters, CancellationToken token = default)
        {
            var (totalRegions, regions) = await _unitOfWork.Regions.GetAsync(queryParameters.Pagination, queryParameters.Sorting, token: token);

            return _mapper.Map<IEnumerable<RegionDto>>(regions)
                .ToPagedResponse(queryParameters.Pagination, totalRegions);
        }

        public async Task<Response<RegionDto>> FindByIdAsync(int id, CancellationToken token = default)
        {
            var region = await _unitOfWork.Regions.FindByIdAsync(id, token);

            return _mapper.Map<RegionDto>(region).ToResponse();
        }

        public async Task<Response<RegionDto>> CreateAsync(RegionDto regionDto, CancellationToken token = default)
        {
            var region = _mapper.Map<Region>(regionDto);

            await _unitOfWork.Regions.AddAsync(region, token);
            await _unitOfWork.CompleteAsync();

            regionDto.RegionId = region.RegionId;

            return regionDto.ToResponse();            
        }

        public async Task<Response<RegionDto>> UpdateAsync(RegionDto regionDto, CancellationToken token = default)
        {
            var regionInDb = 
                await _unitOfWork.Regions.FindByIdAsync(regionDto.RegionId, token) ?? throw new ItemNotFoundException<int>(regionDto.RegionId);
            _mapper.Map(regionDto, regionInDb);
            await _unitOfWork.CompleteAsync();

            return regionDto.ToResponse();
        }

        public async Task<Response<IEnumerable<RegionDto>>> DeleteAsync(int[] ids, CancellationToken token = default)
        {
            var regionsToRemove = (await _unitOfWork.Regions.GetAsync(Pagination.NoPagination(), Sorting.NoSorting(), r => ids.Contains(r.RegionId), token)).items;

            foreach (var region in regionsToRemove)
            {
                _unitOfWork.Regions.Remove(region);
            }
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<RegionDto>>(regionsToRemove).ToResponse();
        }
    }
}
