using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Dtos;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;

namespace Northwind.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerDemographicsController : ControllerBase
    {
        private readonly ICustomerDemographicService _customerDemographicService;

        public CustomerDemographicsController(ICustomerDemographicService customerDemographicService)
        {
            _customerDemographicService = customerDemographicService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerDemographic([FromQuery] PaginationQuery paginationQuery)
        {
            var response = await _customerDemographicService.GetAsync(paginationQuery);

            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetCustomerDemographic")]
        public async Task<IActionResult> GetCustomerDemographic(string id)
        {
            var response = await _customerDemographicService.FindByIdAsync(id);

            if (response.Data == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerDemographic(CustomerDemographicDto customerDemographicDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _customerDemographicService.CreateAsync(customerDemographicDto);

            return CreatedAtAction("GetCustomerDemographic", new { id = response.Data.CustomerTypeId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerDemographic(string id, CustomerDemographicDto customerDemographicDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customerDemographicDto.CustomerTypeId)
            {
                return BadRequest(ModelState);
            }

            if (!await _customerDemographicService.IsExists(id))
            {
                return NotFound();
            }

            var response = await _customerDemographicService.UpdateAsync(customerDemographicDto);

            return Ok(response);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteCustomerDemographics([FromQuery] string[] ids)
        {
            if (!await _customerDemographicService.AreExists(ids))
            {
                return NotFound();
            }

            var response = await _customerDemographicService.DeleteAsync(ids);

            return Ok(response);
        }
    }
}
