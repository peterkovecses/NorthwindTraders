using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Queries;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class RegionService : IRegionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginatedUriService _uriService;

        public RegionService(IUnitOfWork unitOfWork, IMapper mapper, IPaginatedUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public async Task<PagedResponse<RegionDto>> GetAsync(QueryParameters queryParameters)
        {
            var (totalItems, regions) = await _unitOfWork.Regions.GetAsync(queryParameters.Pagination, queryParameters.Sorting);
            queryParameters.SetPaginationIfNull(totalItems);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<RegionDto>>(regions)
                .ToPagedResponse(queryParameters.Pagination, totalItems, next, previous);
        }

        public async Task<Response<RegionDto>> FindByIdAsync(int id)
        {
            var region = await _unitOfWork.Regions.FindByIdAsync(id);

            return _mapper.Map<RegionDto>(region).ToResponse();
        }

        public async Task<Response<RegionDto>> CreateAsync(RegionDto regionDto)
        {
            var region = _mapper.Map<Region>(regionDto);

            await _unitOfWork.Regions.AddAsync(region);
            await _unitOfWork.CompleteAsync();

            regionDto.RegionId = region.RegionId;

            return regionDto.ToResponse();            
        }

        public async Task<Response<RegionDto>> UpdateAsync(RegionDto regionDto)
        {
            var regionInDb = await _unitOfWork.Regions.FindByIdAsync(regionDto.RegionId);

            _mapper.Map(regionDto, regionInDb);
            await _unitOfWork.CompleteAsync();

            return regionDto.ToResponse();
        }

        public async Task<Response<IEnumerable<RegionDto>>> DeleteAsync(int[] ids)
        {
            var regions = (await _unitOfWork.Regions.GetAsync(predicate: r => ids.Contains(r.RegionId))).items;

            _unitOfWork.Regions.Remove(regions);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<RegionDto>>(regions).ToResponse();
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Regions.FindByIdAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Regions.GetAsync(predicate: r => ids.Contains(r.RegionId))).items.Count() == ids.Length;
        }
    }
}
