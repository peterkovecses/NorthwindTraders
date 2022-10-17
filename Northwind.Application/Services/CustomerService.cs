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

        public async Task<PagedResponse<CustomerDto>> GetAsync(QueryParameters<CustomerFilter> queryParameters, CancellationToken token = default)
        {
            var result = await _unitOfWork.Customers.GetAsync(queryParameters.Pagination, queryParameters.Sorting, token: token);
            queryParameters.SetPaginationIfNull(result.TotalItems);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<CustomerDto>>(result.Items)
                .ToPagedResponse(queryParameters.Pagination, result.TotalItems, next, previous);
        }

        public async Task<Response<CustomerDto>> FindByIdAsync(string id, CancellationToken token = default)
        {
            var customer = await _unitOfWork.Customers.FindByIdAsync(id, token);

            return _mapper.Map<CustomerDto>(customer).ToResponse();
        }

        public async Task<Response<CustomerDto>> CreateAsync(CustomerDto customerDto, CancellationToken token = default)
        {
            var customer = _mapper.Map<Customer>(customerDto);

            await _unitOfWork.Customers.AddAsync(customer, token);
            await _unitOfWork.CompleteAsync();

            customerDto.CustomerId = customer.CustomerId;

            return customerDto.ToResponse();
        }

        public async Task<Response<CustomerDto>> UpdateAsync(CustomerDto customerDto, CancellationToken token = default)
        {
            var customerInDb = await _unitOfWork.Customers.FindByIdAsync(customerDto.CustomerId, token);

            customerInDb.CompanyName = customerDto.CompanyName;
            customerInDb.ContactName = customerDto.ContactName;
            customerDto.ContactTitle = customerDto.ContactTitle;
            customerInDb.Address = customerDto.Address;
            customerInDb.City = customerDto.City;
            customerInDb.Region = customerDto.Region;
            customerInDb.PostalCode = customerDto.PostalCode;
            customerInDb.Country = customerDto.Country;
            customerInDb.Phone = customerDto.Phone;
            customerInDb.Fax = customerDto.Fax;

            await _unitOfWork.CompleteAsync();

            return customerDto.ToResponse();
        }

        public async Task<Response<IEnumerable<CustomerDto>>> DeleteAsync(string[] ids, CancellationToken token = default)
        {
            var customers = (await _unitOfWork.Customers.GetAsync(predicate: c => ids.Contains(c.CustomerId), token: token)).Items;

            _unitOfWork.Customers.Remove(customers);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<CustomerDto>>(customers).ToResponse();
        }

        public async Task<bool> IsExists(string id, CancellationToken token = default)
        {
            return await _unitOfWork.Customers.FindByIdAsync(id, token) != null;
        }

        public async Task<bool> AreExists(string[] ids, CancellationToken token = default)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Customers.GetAsync(predicate: c => ids.Contains(c.CustomerId), token: token)).Items.Count() == ids.Length;
        }
    }
}
