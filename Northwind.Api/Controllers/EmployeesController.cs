﻿using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Dtos;

namespace Northwind.Api.Controllers
{
    [Route("employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IPaginatedUriService _uriService;

        public EmployeesController(IEmployeeService employeeService, IPaginatedUriService uriService)
        {
            _employeeService = employeeService;
            _uriService = uriService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] PaginationQuery paginationQuery)
        {
            var response = await _employeeService.GetAllAsync(paginationQuery);

            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var response = await _employeeService.GetAsync(id);

            if (response.Data == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeDto employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.CreateAsync(employee);

            return CreatedAtAction("GetEmployee", new { id = response.Data.EmployeeId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeDto employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.EmployeeId)
            {
                return BadRequest(ModelState);
            }

            if (!await _employeeService.IsExists(id))
            {
                return NotFound();
            }

            var response = await _employeeService.UpdateAsync(employee);

            return Ok(response);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteEmployees([FromQuery] int[] ids)
        {
            if (!await _employeeService.AreExists(ids))
            {
                return NotFound();
            }

            var response = await _employeeService.DeleteRangeAsync(ids);

            return Ok(response);
        }
    }
}
