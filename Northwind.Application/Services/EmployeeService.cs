﻿using AutoMapper;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Dtos;
using Northwind.Domain.Common.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAsync()
        {
            var employees = await _unitOfWork.Employees.GetAsync();

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto>? GetByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<int> CreateAsync(EmployeeDto employeeDto)
        {
            var employee = _mapper.Map<Employee>(employeeDto);

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.CompleteAsync();

            return employee.EmployeeId;
        }

        public async Task UpdateAsync(EmployeeDto employeeDto)
        {
            var employeeInDb = await _unitOfWork.Employees.GetByIdAsync(employeeDto.EmployeeId);

            _mapper.Map(employeeDto, employeeInDb);

            await _unitOfWork.CompleteAsync();
        }

        public async Task<EmployeeDto> DeleteAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);

            _unitOfWork.Employees.Remove(employee);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<IEnumerable<EmployeeDto>> DeleteRangeAsync(int[] ids)
        {
            var employees = await _unitOfWork.Employees.FindAsync(e => ids.Contains(e.EmployeeId));

            _unitOfWork.Employees.RemoveRange(employees);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<bool> IsExists(int id)
        {
            return await _unitOfWork.Employees.GetByIdAsync(id) != null;
        }

        public async Task<bool> AreExists(int[] ids)
        {
            ids = ids.Distinct().ToArray();
            return (await _unitOfWork.Employees.FindAsync(e => ids.Contains(e.EmployeeId))).Count() == ids.Length;
        }
    }
}
