using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.Migrations;
using KASHOP.DAL.Models;
using KASHOP.UI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace KASHOP.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService, IStringLocalizer<SharedResources> localizer)
        {
            _orderService = orderService;
            _localizer = localizer;
        }
        [HttpGet()]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _orderService.GetUserOrders(userId);
            return Ok(new { data = orders});
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserOrder( int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _orderService.GetUserOrder(id, userId);
           
            return Ok(new { data = order });
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _orderService.CancelOrder(id, userId);
            if (!result)
            {
                return BadRequest(new { message = _localizer["OrderCancellationFailed"] });
            }
            return Ok(new { message = _localizer["OrderCancelledSuccessfully"] });
        }

        [HttpGet("admin")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders([FromQuery]OrderStatusEnum status = OrderStatusEnum.Pending )
        {
            var orders = await _orderService.GetAllOrders(status);
            return Ok(new { data = orders });
        }

        [HttpPatch("admin/{id}/status")]
        public async Task<IActionResult> ChangeOrderStatus(int id, [FromBody] ChangeOrderStatusRequest status)
        {
            var result = await _orderService.ChangeOrderStatus(id, status);
            if (!result)
            {
                return BadRequest(new { message = _localizer["OrderStatusChangeFailed"] });
            }
            return Ok(new { message = _localizer["OrderStatusChangedSuccessfully"] });
        }
    }
}
