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
    public class CustomerDemographicService : ICustomerDemographicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginatedUriService _uriService;

        public CustomerDemographicService(IUnitOfWork unitOfWork, IMapper mapper, IPaginatedUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public async Task<PagedResponse<CustomerDemographicDto>> GetAsync(QueryParameters<CustomerDemographicFilter> queryParameters)
        {
            var result = await _unitOfWork.CustomerDemographics.GetAsync(queryParameters.Pagination, queryParameters.Sorting);
            queryParameters.SetPaginationIfNull(result.TotalItems);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<CustomerDemographicDto>>(result.Items)
                .ToPagedResponse(queryParameters.Pagination, result.TotalItems, next, previous);
        }

        public async Task<Response<CustomerDemographicDto>> FindByIdAsync(string id)
        {
            var customerDemographic = await _unitOfWork.CustomerDemographics.FindByIdAsync(id);

            return _mapper.Map<CustomerDemographicDto>(customerDemographic).ToResponse();
        }

        public async Task<Response<CustomerDemographicDto>> CreateAsync(CustomerDemographicDto customerDemographicDto)
        {
            var customerDemographic = _mapper.Map<CustomerDemographic>(customerDemographicDto);

            await _unitOfWork.CustomerDemographics.AddAsync(customerDemographic);
            await _unitOfWork.CompleteAsync();

            customerDemographicDto.CustomerTypeId = customerDemographic.CustomerTypeId;

            return customerDemographicDto.ToResponse();
        }

        public async Task<Response<CustomerDemographicDto>> UpdateAsync(CustomerDemographicDto customerDemographicDto)
        {
            var customerDemographicInDb = await _unitOfWork.CustomerDemographics.FindByIdAsync(customerDemographicDto.CustomerTypeId);

            customerDemographicInDb.CustomerDesc = customerDemographicDto.CustomerDesc;
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CustomerDemographicDto>(customerDemographicInDb).ToResponse();
        }

        public async Task<Response<IEnumerable<CustomerDemographicDto>>> DeleteAsync(string[] ids)
        {
            var customerDemographics = (await _unitOfWork.CustomerDemographics.GetAsync(predicate: x => ids.Contains(x.CustomerTypeId))).Items;

            _unitOfWork.CustomerDemographics.Remove(customerDemographics);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<CustomerDemographicDto>>(customerDemographics).ToResponse();
        }

        public async Task<bool> IsExists(string id)
        {
            return await _unitOfWork.CustomerDemographics.FindByIdAsync(id) != null;
        }

        public async Task<bool> AreExists(string[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.CustomerDemographics.GetAsync(predicate: x => ids.Contains(x.CustomerTypeId))).Items.Count() == ids.Length;
        }
    }
}
