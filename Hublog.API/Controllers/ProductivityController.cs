using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductivityController : ControllerBase
    {
        private readonly IProductivityService _productivityService;
        public ProductivityController(IProductivityService productivityService) 
        {
            _productivityService = productivityService;
        }

        [HttpGet("Category")]
        public async Task<IActionResult> GetCategory([FromQuery] string categoryName = "")
        {
            var categories = await _productivityService.GetCategoryProductivity(categoryName);

            if (categories == null || !categories.Any())
                return NotFound(new { message = "No categories found." });

            return Ok(categories);
        }

        [HttpPut("UpdateProductivityId")]
        public async Task<IActionResult> UpdateProductivityId(int categoryId, int? productivityId)
        {
            var isUpdated = await _productivityService.UpdateProductivityId(categoryId, productivityId);

            if (isUpdated)
                return Ok(new { message = "ProductivityId updated successfully." });
            else
                return NotFound(new { message = "Category not found." });
        }
        [HttpGet("GetImbuildAppsAndUrls")]
        public async Task<IActionResult> GetImbuildAppsAndUrls()
        {
            var categories = await _productivityService.GetImbuildAppsAndUrls();
            return Ok(categories);
        }
    }
}
