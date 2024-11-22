using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Service.Interface;
using Hublog.Service.Services;
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
        [HttpGet("GetByIdImbuildAppsAndUrls")]
        public async Task<IActionResult> GetByIdImbuildAppsAndUrls(int id)
        {
            var categories = await _productivityService.GetByIdImbuildAppsAndUrls(id);
            return Ok(categories);
        }
        [HttpPut("InsertImbuildAppsAndUrls/{id}")]
        public async Task<IActionResult> InsertImbuildAppsAndUrls(int id, [FromBody] MappingModel model)
        {
            if (model.CategoryId == null)
            {
                return BadRequest("CategoryId is required.");
            }

            var result = await _productivityService.InsertImbuildAppsAndUrls(id, model);
            return Ok(result);
        }

        [HttpGet("GetProductivityBreakDown")]
        public async Task<IActionResult> GetProductivityDurations(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var result = await _productivityService.GetProductivityDurations(organizationId, teamId, userId, fromDate, toDate);
            return Ok(result);
            
        }
        

    }
}
