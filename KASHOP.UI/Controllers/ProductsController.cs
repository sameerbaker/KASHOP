using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.UI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace KASHOP.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IProductService _productService;

        public ProductsController(IProductService productService, IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
            _productService = productService;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductAsync();

            return Ok(new
            {
                data = products
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var product = await _productService.GetProduct(p=>p.Id == id);
            if (product == null) return NotFound();

            return Ok(new
            {
                data = product
            });
        }

        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] ProductRequest request) 
        {
            await _productService.CreateProduct(request);
            return Ok();
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateRequest request)
        {
            var updated = await _productService.UpdateProduct(id, request);
            if (!updated) return BadRequest();

            return Ok();
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteProduct(id);
            if (!deleted) return BadRequest();

            return Ok();
        }
    }
}
