using KASHOP.BLL.Service;
using KASHOP.DAL;
using KASHOP.DAL.DTO.Request;
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
    public class CartsController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ICartService _cartService;
        public CartsController(ICartService cartService, IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
            _cartService = cartService;
        }

        [HttpPost("")]

        public async Task<IActionResult> AddToCart(AddToCartRequest request)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.AddToCart(request, UserId);

            if (!result) return BadRequest();
            return Ok(new
            {
                message = _localizer["Success"].Value,

            });

        }

        [HttpGet("")]
        public async Task<IActionResult> GetCart()
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var items = await _cartService.GetCart(UserId);
            return Ok(new { data = items });
        }

        [HttpPatch("{productId}")]
        public async Task<IActionResult> UpdateQuantity([FromRoute]int productId, [FromBody] UpdateCartRequest request)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var updated = await _cartService.UpdateQuantity(productId, request.Count, UserId);
            if (!updated) return BadRequest();
            return Ok(new { message = _localizer["Success"].Value });

        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveItem([FromRoute] int productId)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var removed = await _cartService.RemoveItem(productId, UserId);
            if (!removed) return BadRequest();
            return Ok(new { message = _localizer["Success"].Value });
        }


    }

}
