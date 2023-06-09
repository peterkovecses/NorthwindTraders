﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Api.Extensions;
using Northwind.Application.Dtos;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;

namespace Northwind.Api.Controllers
{
    [Route("customerdemographics")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CustomerDemographicsController : ApiControllerBase
    {
        private readonly ICustomerDemographicService _customerDemographicService;

        public CustomerDemographicsController(
            ICustomerDemographicService customerDemographicService, 
            ILogger<CustomerDemographicsController> logger) 
            : base(logger)
        {
            _customerDemographicService = customerDemographicService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerDemographic(
            [FromQuery] QueryParameters<CustomerDemographicFilter, CustomerDemographic> queryParameters, 
            CancellationToken token)
        {
            var response = (await _customerDemographicService.GetAsync(queryParameters, token)).SetNavigation(BaseUri); ;

            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetCustomerDemographic")]
        public async Task<IActionResult> GetCustomerDemographic(string id, CancellationToken token)
        {
            var response = await _customerDemographicService.FindByIdAsync(id, token);

            return GetResult(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerDemographic(CustomerDemographicDto customerDemographic, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCustomerDemographic = (await _customerDemographicService.FindByIdAsync(customerDemographic.CustomerTypeId, token)).Data;

            if (existingCustomerDemographic != null)
            {
                return Conflict("A CustomerDemographic with the same CustomerTypeId already exists.");
            }

            var response = await _customerDemographicService.CreateAsync(customerDemographic, token);

            return CreatedAtAction(nameof(GetCustomerDemographic), new { id = response.Data.CustomerTypeId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerDemographic(
            string id, 
            CustomerDemographicDto customerDemographic, 
            CancellationToken token)
        {
            if (id != customerDemographic.CustomerTypeId)
            {
                ModelState.AddModelError("id", IdsNotMatchMessage);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _customerDemographicService.UpdateAsync(customerDemographic, token);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerDemographics(string id, CancellationToken token)
        {
            await _customerDemographicService.DeleteAsync(id, token);

            return Ok();
        }
    }
}
