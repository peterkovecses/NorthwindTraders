using Microsoft.AspNetCore.Mvc;
using Northwind.Api.Extensions;
using Northwind.Application.Dtos;
using Northwind.Application.Exceptions;
using Northwind.Application.Extensions;
using Northwind.Application.Interfaces.Services;
using Northwind.Application.Models;
using Northwind.Application.Models.Filters;

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
        public async Task<IActionResult> GetOrderDetails([FromQuery] QueryParameters<OrderDetailFilter> queryParameters, CancellationToken token)
        {
            var response = (await _orderDetailService.GetAsync(queryParameters, token)).SetNavigation(BaseUri); ;

            return Ok();
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

            var response = await _orderDetailService.CreateAsync(orderDetail, token);

            return CreatedAtAction("GetOrderDetail", new { orderId = response.Data.OrderId, productId = response.Data.ProductId }, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrderDetail(int orderId, int productId, OrderDetailDto orderDetail, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (orderId != orderDetail.OrderId || productId != orderDetail.ProductId)
            {
                return BadRequest(ModelState);
            }

            Response<OrderDetailDto> response;
            try
            {
            response = await _orderDetailService.UpdateAsync(orderDetail, token);
            }
            catch (ItemNotFoundException ex)
            {
                _logger.LogError(ex);
                return NotFound();
            }

            return Ok(response);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteOrderDetail(OrderDetailKey[] ids, CancellationToken token)
        {
            var response = await _orderDetailService.DeleteAsync(ids, token);

            return Ok(response);
        }
    }
}
