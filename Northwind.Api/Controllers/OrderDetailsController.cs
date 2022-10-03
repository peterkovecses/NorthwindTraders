using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Extensions;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Common.Queries;
using Northwind.Application.Dtos;
using Northwind.Application.Common.Models;

namespace Northwind.Api.Controllers
{
    [Route("orderdetails")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;
        private readonly IPaginatedUriService _uriService;

        public OrderDetailsController(IOrderDetailService orderDetailService, IPaginatedUriService uriService)
        {
            _orderDetailService = orderDetailService;
            _uriService = uriService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails([FromQuery] PaginationQuery paginationQuery)
        {
            var response = await _orderDetailService.GetAllAsync(paginationQuery);

            return Ok(response);
        }

        [HttpGet("detail", Name = "GetOrderDetail")]
        public async Task<IActionResult> GetOrderDetail(int orderId, int productId)
        {
            var key = new OrderDetailKey(orderId, productId);

            var response = await _orderDetailService.GetAsync(key);

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

            var response = await _orderDetailService.DeleteRangeAsync(ids);

            return Ok(response);
        }
    }
}
