using AutoMapper;
using Northwind.Application.Dtos;
using Northwind.Application.Exceptions;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Application.Services.PredicateBuilders;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly EmployeePredicateBuilder _predicateBuilder;

        public EmployeeService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            EmployeePredicateBuilder predicateBuilder)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _predicateBuilder = predicateBuilder;
        }

        public async Task<PagedResponse<EmployeeDto>> GetAsync(QueryParameters<EmployeeFilter> queryParameters, CancellationToken token = default)
        {            
            var predicate = _predicateBuilder.GetPredicate(queryParameters);
            (int totalEmployees, IEnumerable<Employee> employees) = await _unitOfWork.Employees.GetAsync(queryParameters.Pagination, queryParameters.Sorting, predicate, token);
            queryParameters.SetPaginationIfNull(totalEmployees);

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees)
                .ToPagedResponse(queryParameters.Pagination, totalEmployees);
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
            var employeeInDb = 
                await _unitOfWork.Employees.FindByIdAsync(employeeDto.EmployeeId, token) ?? throw new ItemNotFoundException(employeeDto.EmployeeId);
            _mapper.Map(employeeDto, employeeInDb);
            await _unitOfWork.CompleteAsync();

            return employeeDto.ToResponse();
        }

        public async Task<Response<IEnumerable<EmployeeDto>>> DeleteAsync(int[] ids, CancellationToken token)
        {
            var employeesToRemove = (await _unitOfWork.Employees.GetAsync(predicate: e => ids.Contains(e.EmployeeId), token: token)).items;

            foreach (var employee in employeesToRemove)
            {
                _unitOfWork.Employees.Remove(employee);
            }
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<EmployeeDto>>(employeesToRemove).ToResponse();
        }       
    }
}
