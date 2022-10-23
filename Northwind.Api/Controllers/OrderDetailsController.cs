using Microsoft.AspNetCore.Mvc;
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
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;
        private readonly IPaginatedUriService _uriService;
        private readonly ILogger<OrderDetailsController> _logger;

        public OrderDetailsController(IOrderDetailService orderDetailService, IPaginatedUriService uriService, ILogger<OrderDetailsController> logger)
        {
            _orderDetailService = orderDetailService;
            _uriService = uriService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails([FromQuery] QueryParameters<OrderDetailFilter> queryParameters, CancellationToken token)
        {
            queryParameters.SetParameters(nameof(OrderDetailDto.OrderId));
            var response = await _orderDetailService.GetAsync(queryParameters, token);
            (response.NextPage, response.PreviousPage) = _uriService.GetNavigations(queryParameters.Pagination);

            return Ok(response);
        }

        [HttpGet("detail", Name = "GetOrderDetail")]
        public async Task<IActionResult> GetOrderDetail(int orderId, int productId, CancellationToken token)
        {
            var key = new OrderDetailKey(orderId, productId);

            var response = await _orderDetailService.FindByIdAsync(key, token);

            if (response.HasData)
            {
                return Ok(response);
            }

            return NotFound();
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
