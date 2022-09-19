using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Dtos;

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
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _employeeService.GetAsync();

            return Ok(employees);
        }

        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            employeeDto.EmployeeId = await _employeeService.CreateAsync(employeeDto);

            return CreatedAtAction("GetEmployee", new { id = employeeDto.EmployeeId }, employeeDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employeeDto.EmployeeId)
            {
                return BadRequest(ModelState);
            }

            if (!_employeeService.IsExists(id).Result)
            {
                return NotFound();
            }

            try
            {
                await _employeeService.UpdateAsync(employeeDto);

                return Ok(employeeDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _employeeService.IsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteEmployees([FromQuery] int[] ids)
        {
            if (!await _employeeService.AreExists(ids))
            {
                return NotFound();
            }

            var employees = await _employeeService.DeleteRangeAsync(ids);

            return Ok(employees);
        }
    }
}
