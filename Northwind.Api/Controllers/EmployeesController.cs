using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Dtos;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;

namespace Northwind.Api.Controllers
{
    [Route("employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] PaginationQuery paginationQuery)
        {
            var response = await _employeeService.GetAsync(paginationQuery);

            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var response = await _employeeService.FindByIdAsync(id);

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

            var response = await _employeeService.DeleteAsync(ids);

            return Ok(response);
        }
    }
}
