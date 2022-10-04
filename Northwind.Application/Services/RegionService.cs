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

        public async Task<Response<IEnumerable<RegionDto>>> GetAllAsync()
        {
            var regions = await _unitOfWork.Regions.GetAllAsync();
            return _mapper.Map<IEnumerable<RegionDto>>(regions).ToResponse();
        }

        public async Task<PagedResponse<RegionDto>> GetAllAsync(PaginationQuery paginationQuery)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var (totalItems, regions) = await _unitOfWork.Regions.GetAllAsync(paginationFilter);
            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return _mapper.Map<IEnumerable<RegionDto>>(regions)
                .ToPagedResponse(paginationQuery, totalItems, next, previous);
        }

        public async Task<Response<RegionDto>> GetAsync(int id)
        {
            var region = await _unitOfWork.Regions.GetAsync(id);

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
            var regionInDb = await _unitOfWork.Regions.GetAsync(regionDto.RegionId);

            _mapper.Map(regionDto, regionInDb);
            await _unitOfWork.CompleteAsync();

            return regionDto.ToResponse();
        }

        public async Task<Response<RegionDto>> DeleteAsync(int id)
        {
            var region = await _unitOfWork.Regions.GetAsync(id);

            _unitOfWork.Regions.Remove(region);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<RegionDto>(region).ToResponse();
        }

        public async Task<Response<IEnumerable<RegionDto>>> DeleteRangeAsync(int[] ids)
        {
            var regions = await _unitOfWork.Regions.FindAllAsync(r => ids.Contains(r.RegionId));

            _unitOfWork.Regions.RemoveRange(regions);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<RegionDto>>(regions).ToResponse();
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Regions.GetAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Regions.FindAllAsync(r => ids.Contains(r.RegionId))).Count() == ids.Length;
        }
    }
}
