using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Dtos;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;

namespace Northwind.Api.Controllers
{
    [Route("customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] QueryParameters<CustomerFilter> queryParameters, CancellationToken token)
        {
            var response = await _customerService.GetAsync(queryParameters, token);

            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> GetCustomer(string id, CancellationToken token)
        {
            var response = await _customerService.FindByIdAsync(id, token);

            if (response.Data == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CustomerDto customer, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _customerService.CreateAsync(customer, token);

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(string id, CustomerDto customer, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.CustomerId)
            {
                return BadRequest(ModelState);
            }

            if (! await _customerService.IsExists(id, token))
            {
                return NotFound();
            }

            var response = await _customerService.UpdateAsync(customer, token);

            return Ok(response);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteCustomers([FromQuery] string[] ids, CancellationToken token)
        {
            if (!await _customerService.AreExists(ids, token))
            {
                return NotFound();
            }

            var response = await _customerService.DeleteAsync(ids, token);

            return Ok(response);
        }
    }
}
