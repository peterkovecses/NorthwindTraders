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
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginatedUriService _uriService;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IPaginatedUriService uriService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public async Task<PagedResponse<CustomerDto>> GetAllAsync(PaginationQuery? paginationQuery = null)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var customers = await _unitOfWork.Customers.GetAllAsync(paginationFilter);

            var response = _mapper.Map<IEnumerable<CustomerDto>>(customers).ToPagedResponse();

            if (paginationQuery == null)
            {
                return response;
            }

            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return response.SetPagination(paginationQuery, next, previous);
        }

        public async Task<Response<CustomerDto>> GetAsync(string id)
        {
            var customer = await _unitOfWork.Customers.GetAsync(id);

            return _mapper.Map<CustomerDto>(customer).ToResponse();
        }

        public async Task<Response<CustomerDto>> CreateAsync(CustomerDto customerDto)
        {
            var customer = _mapper.Map<Customer>(customerDto);

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.CompleteAsync();

            customerDto.CustomerId = customer.CustomerId;

            return customerDto.ToResponse();
        }

        public async Task<Response<CustomerDto>> UpdateAsync(CustomerDto customerDto)
        {
            var customerInDb = await _unitOfWork.Customers.GetAsync(customerDto.CustomerId);

            _mapper.Map(customerDto, customerInDb);
            await _unitOfWork.CompleteAsync();

            return customerDto.ToResponse();
        }

        public async Task<Response<CustomerDto>> DeleteAsync(string id)
        {
            var customer = await _unitOfWork.Customers.GetAsync(id);

            _unitOfWork.Customers.Remove(customer);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CustomerDto>(customer).ToResponse();
        }

        public async Task<Response<IEnumerable<CustomerDto>>> DeleteRangeAsync(string[] ids)
        {
            var customers = await _unitOfWork.Customers.FindAllAsync(c => ids.Contains(c.CustomerId));

            _unitOfWork.Customers.RemoveRange(customers);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<CustomerDto>>(customers).ToResponse();
        }

        public async Task<bool> IsExists(string id)
        {
            return await _unitOfWork.Customers.GetAsync(id) != null;
        }

        public async Task<bool> AreExists(string[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Customers.FindAllAsync(c => ids.Contains(c.CustomerId))).Count() == ids.Length;
        }
    }
}
