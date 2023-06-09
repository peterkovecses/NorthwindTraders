﻿using Microsoft.AspNetCore.Mvc;
using Northwind.Api.Extensions;
using Northwind.Application.Dtos;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;
using Northwind.Domain.Entities;

namespace Northwind.Api.Controllers
{
    [Route("orderdetails")]
    [ApiController]
    public class OrderDetailsController : ApiControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailsController(
            IOrderDetailService orderDetailService, 
            ILogger<OrderDetailsController> logger)
            : base(logger)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails([FromQuery] QueryParameters<OrderDetailFilter, OrderDetail> queryParameters, CancellationToken token)
        {
            var response = (await _orderDetailService.GetAsync(queryParameters, token)).SetNavigation(BaseUri);

            return Ok(response);
        }

        [HttpGet("detail", Name = "GetOrderDetail")]
        public async Task<IActionResult> GetOrderDetail(int orderId, int productId, CancellationToken token)
        {
            var key = new OrderDetailKey(orderId, productId);
            var response = await _orderDetailService.FindByIdAsync(key, token);

            return GetResult(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderDetail(OrderDetailDto orderDetail, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingOrderDetail = (await _orderDetailService.FindByIdAsync(new OrderDetailKey(orderDetail.OrderId, orderDetail.ProductId), token)).Data;

            if (existingOrderDetail != null)
            {
                return Conflict("An OrderDetail with the same OrderId and ProductId already exists.");
            }

            var response = await _orderDetailService.CreateAsync(orderDetail, token);

            return CreatedAtAction(nameof(GetOrderDetail), new { orderId = response.Data.OrderId, productId = response.Data.ProductId }, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrderDetail(int orderId, int productId, OrderDetailDto orderDetail, CancellationToken token)
        {
            if (orderId != orderDetail.OrderId || productId != orderDetail.ProductId)
            {
                ModelState.AddModelError("id", IdsNotMatchMessage);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _orderDetailService.UpdateAsync(orderDetail, token);           

            return Ok(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrderDetail(OrderDetailKey id, CancellationToken token)
        {
            await _orderDetailService.DeleteAsync(id, token);

            return Ok();
        }
    }
}
