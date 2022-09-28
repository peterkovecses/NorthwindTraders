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
            var orderDetails = await _orderDetailService.GetAllAsync(paginationQuery);

            if (paginationQuery == null || paginationQuery.PageNumber < 1 || paginationQuery.PageSize < 1)
            {
                return Ok(orderDetails.ToPagedResponse());
            }

            var (next, previous) = _uriService.GetNavigations(paginationQuery);

            return Ok(orderDetails.ToPagedResponse().SetPagination(paginationQuery, next, previous));
        }

        [HttpGet("detail", Name = "GetOrderDetail")]
        public async Task<IActionResult> GetOrderDetail(int orderId, int productId)
        {
            var key = new OrderDetailKey(orderId, productId);

            var orderDetail = await _orderDetailService.GetAsync(key);

            if (orderDetail == null)
            {
                return NotFound();
            }

            return Ok(orderDetail.ToResponse());
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderDetail(OrderDetailDto orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var key = await _orderDetailService.CreateAsync(orderDetail);
            orderDetail.OrderId = key.OrderId;
            orderDetail.ProductId = key.ProductId;

            return CreatedAtAction("GetOrderDetail", new { id = key }, orderDetail.ToResponse());
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

            try
            {
                await _orderDetailService.UpdateAsync(orderDetail);

                return Ok(orderDetail.ToResponse());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _orderDetailService.IsExists(key))
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
        public async Task<IActionResult> DeleteOrderDetail(OrderDetailKey[] ids)
        {
            if (!await _orderDetailService.AreExists(ids))
            {
                return NotFound();
            }

            var orderDetails = await _orderDetailService.DeleteRangeAsync(ids);

            return Ok(orderDetails.ToResponse());
        }
    }
}
