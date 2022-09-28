using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Extensions;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Dtos;

namespace Northwind.Api.Controllers
{
    [Route("customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IPaginatedUriService _uriService;

        public CustomersController(ICustomerService customerService, IPaginatedUriService uriService)
        {
            _customerService = customerService;
            _uriService = uriService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] PaginationQuery paginationQuery)
        {
            var customers = await _customerService.GetAllAsync(paginationQuery);

            if (paginationQuery == null || paginationQuery.PageNumber < 1 || paginationQuery.PageSize < 1)
            {
                return Ok(customers.ToPagedResponse());
            }

            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return Ok(customers.ToPagedResponse().SetPagination(paginationQuery, next, previous));
        }

        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> GetCustomer(string id)
        {
            var customer = await _customerService.GetAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer.ToResponse());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CustomerDto customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            customer.CustomerId = await _customerService.CreateAsync(customer);

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer.ToResponse());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(string id, CustomerDto customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.CustomerId)
            {
                return BadRequest(ModelState);
            }

            if (!_customerService.IsExists(id).Result)
            {
                return NotFound();
            }

            try
            {
                await _customerService.UpdateAsync(customer);

                return Ok(customer.ToResponse());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _customerService.IsExists(id))
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
        [Route("delete")]
        public async Task<IActionResult> DeleteCustomers([FromQuery] string[] ids)
        {
            if (!await _customerService.AreExists(ids))
            {
                return NotFound();
            }

            var customers = await _customerService.DeleteRangeAsync(ids);

            return Ok(customers.ToResponse());
        }
    }
}
