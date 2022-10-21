using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Dtos;
using Northwind.Application.Exceptions;
using Northwind.Application.Extensions;
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
        private readonly IPaginatedUriService _uriService;

        public CustomersController(ICustomerService customerService, IPaginatedUriService paginatedUriService)
        {
            _customerService = customerService;
            _uriService = paginatedUriService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] QueryParameters<CustomerFilter> queryParameters, CancellationToken token)
        {
            queryParameters.SetParameters(nameof(CustomerDto.CustomerId));
            var response = await _customerService.GetAsync(queryParameters, token);
            (response.NextPage, response.PreviousPage) = _uriService.GetNavigations(queryParameters.Pagination);

            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> GetCustomer(string id, CancellationToken token)
        {
            var response = await _customerService.FindByIdAsync(id, token);

            if (response.HasData)
            {
                return Ok(response);
            }

            return NotFound();
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

            Response<CustomerDto> response;
            try
            {
                response = await _customerService.UpdateAsync(customer, token);
            }
            catch (ItemNotFoundException)
            {

                return NotFound();
            }

            return Ok(response);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteCustomers([FromQuery] string[] ids, CancellationToken token)
        {
            var response = await _customerService.DeleteAsync(ids, token);

            return Ok(response);
        }
    }
}
