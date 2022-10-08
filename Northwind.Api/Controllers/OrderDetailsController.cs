using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Dtos;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Queries;

namespace Northwind.Api.Controllers
{
    [Route("orderdetails")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailsController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails([FromQuery] QueryParameters queryParameters)
        {
            var response = await _orderDetailService.GetAsync(queryParameters);

            return Ok(response);
        }

        [HttpGet("detail", Name = "GetOrderDetail")]
        public async Task<IActionResult> GetOrderDetail(int orderId, int productId)
        {
            var key = new OrderDetailKey(orderId, productId);

            var response = await _orderDetailService.FindByIdAsync(key);

            if (response.Data == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderDetail(OrderDetailDto orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _orderDetailService.CreateAsync(orderDetail);

            return CreatedAtAction("GetOrderDetail", new { orderId = response.Data.OrderId, productId = response.Data.ProductId }, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrderDetail(int orderId, int productId, OrderDetailDto orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (orderId != orderDetail.OrderId || productId != orderDetail.ProductId)
            {
                return BadRequest(ModelState);
            }

            var key = new OrderDetailKey(orderId, productId);

            if (!_orderDetailService.IsExists(key).Result)
            {
                return NotFound();
            }

            var response = await _orderDetailService.UpdateAsync(orderDetail);

            return Ok(response);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteOrderDetail(OrderDetailKey[] ids)
        {
            if (!await _orderDetailService.AreExists(ids))
            {
                return NotFound();
            }

            var response = await _orderDetailService.DeleteAsync(ids);

            return Ok(response);
        }
    }
}
