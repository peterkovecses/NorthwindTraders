using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Interfaces.Services.PredicateBuilders;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginatedUriService _uriService;
        private readonly IEmployeePredicateBuilder _predicateBuilder;

        public EmployeeService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IPaginatedUriService uriService, 
            IEmployeePredicateBuilder predicateBuilder)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
            _predicateBuilder = predicateBuilder;
        }

        public async Task<PagedResponse<EmployeeDto>> GetAsync(QueryParameters<EmployeeFilter> queryParameters, CancellationToken token = default)
        {
            RepositoryCollectionResult<Employee> result;

            if (queryParameters.Filter != null)
            {
                var predicate = _predicateBuilder.GetPredicate(queryParameters);

                result = await _unitOfWork.Employees.GetAsync(queryParameters.Pagination, queryParameters.Sorting, predicate, token);
            }
            else
            {
                result = await _unitOfWork.Employees.GetAsync(queryParameters.Pagination, queryParameters.Sorting, token: token);
            }

            queryParameters.SetPaginationIfNull(result.TotalItems);
            var (next, previous) = _uriService.GetNavigations(queryParameters.Pagination);

            return _mapper.Map<IEnumerable<EmployeeDto>>(result.Items)
                .ToPagedResponse(queryParameters.Pagination, result.TotalItems, next, previous);
        }

        public async Task<Response<EmployeeDto>> FindByIdAsync(int id, CancellationToken token)
        {
            var employee = await _unitOfWork.Employees.FindByIdAsync(id, token);

            return _mapper.Map<EmployeeDto>(employee).ToResponse();
        }

        public async Task<Response<EmployeeDto>> CreateAsync(EmployeeDto employeeDto, CancellationToken token = default)
        {
            var employee = _mapper.Map<Employee>(employeeDto);

            await _unitOfWork.Employees.AddAsync(employee, token);
            await _unitOfWork.CompleteAsync();

            employeeDto.EmployeeId = employee.EmployeeId;

            return employeeDto.ToResponse();
        }

        public async Task<Response<EmployeeDto>> UpdateAsync(EmployeeDto employeeDto, CancellationToken token)
        {
            var employeeInDb = await _unitOfWork.Employees.FindByIdAsync(employeeDto.EmployeeId, token);

            _mapper.Map(employeeDto, employeeInDb);
            await _unitOfWork.CompleteAsync();

            return employeeDto.ToResponse();
        }

        public async Task<Response<IEnumerable<EmployeeDto>>> DeleteAsync(int[] ids, CancellationToken token)
        {
            var employees = (await _unitOfWork.Employees.GetAsync(predicate: e => ids.Contains(e.EmployeeId), token: token)).Items;

            _unitOfWork.Employees.Remove(employees);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees).ToResponse();
        }

        public async Task<bool> IsExists(int id, CancellationToken token = default)
        {
            return await _unitOfWork.Employees.FindByIdAsync(id, token) != null;
        }

        public async Task<bool> AreExists(int[] ids, CancellationToken token = default)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Employees.GetAsync(predicate: e => ids.Contains(e.EmployeeId), token: token)).Items.Count() == ids.Length;
        }        
    }
}
