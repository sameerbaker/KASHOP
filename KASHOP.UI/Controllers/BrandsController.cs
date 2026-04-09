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
    public class BrandsController : Controller
    {
        private readonly IBrandService _brandService;
        private readonly IStringLocalizer<SharedResources> _localizer;
        public BrandsController(IBrandService brandService, IStringLocalizer<SharedResources> localizer)
        {
            _brandService = brandService;
            _localizer = localizer;
        }
        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> Create(BrandRequest request)
        {
            var response = await _brandService.CreateBrand(request);
            return Ok(new { message = _localizer["Success"].Value, response });
        }
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var brands = await _brandService.GetAllBrands();
            return Ok(new { data = brands, _localizer["Success"].Value });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _brandService.GetBrand(b => b.Id == id));
        }
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _brandService.DeleteBrand(id);
            if (!deleted) return NotFound(new { message = _localizer["NotFound"].Value });
            return Ok(new { message = _localizer["Deleted"].Value });
        }
    }
}
