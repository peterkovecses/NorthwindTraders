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
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<CustomerDto>> GetAsync(QueryParameters<CustomerFilter, Customer> queryParameters, CancellationToken token = default)
        {
            var (totalCustomers, customers) = await _unitOfWork.Customers.GetAsync(queryParameters.Pagination, queryParameters.Sorting, queryParameters.Filter.GetPredicate(), token);

            return _mapper.Map<IEnumerable<CustomerDto>>(customers)
                .ToPagedResponse(queryParameters.Pagination, totalCustomers);
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
            var customerInDb = 
                await _unitOfWork.Customers.FindByIdAsync(customerDto.CustomerId, token) ?? throw new ItemNotFoundException<string>(customerDto.CustomerId);

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

        public async Task DeleteAsync(string id, CancellationToken token = default)
        {
            var customerToRemove = await _unitOfWork.Customers.FindByIdAsync(id, token);
            _unitOfWork.Customers.Remove(customerToRemove);

            await _unitOfWork.CompleteAsync();
        }
    }
}
