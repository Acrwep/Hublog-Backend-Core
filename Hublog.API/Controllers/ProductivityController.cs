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
            if (categories != null && categories.Any()) 
                return Ok(new { message = "ProductivityId updated successfully.", data = categories });
            else
                return NotFound(new { message = "Categories not found" });
        }
        [HttpGet("GetByIdImbuildAppsAndUrls")]
        public async Task<IActionResult> GetByIdImbuildAppsAndUrls(int id)
        {
            var categories = await _productivityService.GetByIdImbuildAppsAndUrls(id);

            if (categories != null) 
                return Ok(categories);
            else
                return NotFound(new { message = "Category not found" });
        }
        [HttpPut("InsertImbuildAppsAndUrls/{id}")]
        public async Task<IActionResult> InsertImbuildAppsAndUrls(int id, [FromBody] MappingModel model)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }
            if (model == null)
            {
                return BadRequest("Model cannot be null.");
            }

            if (model.CategoryId == null)
            {
                return BadRequest("CategoryId is required.");
            }

            var result = await _productivityService.InsertImbuildAppsAndUrls(id, model);

            if (result == null)
            {
                return NotFound(new { message = "Failed to insert data." });
            }
            return Ok(new { message = "Data inserted successfully", data = result });
        }

        [HttpGet("GetProductivityBreakDown")]
        public async Task<IActionResult> GetProductivityDurations(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            if (organizationId <= 0)
            {
                return BadRequest("Invalid organization ID.");
            }

            if (fromDate == default || toDate == default)
            {
                return BadRequest("Invalid date range.");
            }

            var result = await _productivityService.GetProductivityDurations(organizationId, teamId, userId, fromDate, toDate);

            if (result == null)
            {
                return NotFound(new { message = "No productivity data found for the given parameters." });
            }

            return Ok(result);
        }

        [HttpGet("Teamwise_Productivity")]
        public async Task<IActionResult> TeamwiseProductivity(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            if (organizationId <= 0)
            {
                return BadRequest("Invalid organization ID.");
            }

            if (fromDate == default || toDate == default)
            {
                return BadRequest("Invalid date range.");
            }

            var result = await _productivityService.TeamwiseProductivity(organizationId, teamId, fromDate, toDate);

            if (result == null || !result.Any())
            {
                return NotFound(new { message = "No teamwise productivity data found for the given parameters." });
            }

            return Ok(result);
        }

        [HttpGet("Most&Least_Teamwise_Productivity")]
        public async Task<IActionResult> MostTeamwiseProductivity(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            if (organizationId <= 0)
            {
                return BadRequest("Invalid organization ID.");
            }

            if (fromDate == default || toDate == default)
            {
                return BadRequest("Invalid date range.");
            }

            var result = await _productivityService.MostTeamwiseProductivity(organizationId, teamId, fromDate, toDate);

            if (result == null || !result.Any())
            {
                return NotFound(new { message = "No teamwise productivity data found for the given parameters." });
            }

            return Ok(result);
        }

        [HttpGet("Total_Working_Time")]
        public async Task<IActionResult> GetTotal_Working_Time(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            if (organizationId <= 0)
            {
                return BadRequest("Invalid organization ID.");
            }

            if (fromDate == default || toDate == default)
            {
                return BadRequest("Invalid date range.");
            }

            if (fromDate > toDate)
            {
                return BadRequest("FromDate cannot be greater than ToDate.");
            }
            var result = await _productivityService.GetTotal_Working_Time(organizationId, teamId, userId, fromDate, toDate);

            if (result == null || result.Equals(0))
            {
                return NotFound(new { message = "No total working time data found for the given parameters." });
            }

            return Ok(new { message = "Total working time data fetched successfully.", data = result });
        }

        [HttpGet("GetProductivity_Trend")]
        public async Task<IActionResult> GetProductivity_Trend(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            if (organizationId <= 0)
            {
                return BadRequest("Invalid organization ID.");
            }

            if (fromDate == default || toDate == default)
            {
                return BadRequest("Invalid date range.");
            }

            if (fromDate > toDate)
            {
                return BadRequest("FromDate cannot be greater than ToDate.");
            }

            var result = await _productivityService.GetProductivity_Trend(organizationId, teamId, userId, fromDate, toDate);

            if (result == null || !result.Any()) 
            {
                return NotFound(new { message = "No productivity trend data found for the given parameters." });
            }

            return Ok(new { message = "Productivity trend data fetched successfully.", data = result });
        }

        [HttpGet("GetEmployeeList")]
        public async Task<IActionResult> GetEmployeeList(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            if (organizationId <= 0)
            {
                return BadRequest("Invalid organization ID.");
            }

            if (fromDate == default || toDate == default)
            {
                return BadRequest("Invalid date range.");
            }

            if (fromDate > toDate)
            {
                return BadRequest("FromDate cannot be greater than ToDate.");
            }
            var result = await _productivityService.GetEmployeeList(organizationId, teamId, userId, fromDate, toDate);

            if (result == null || !result.Any()) 
            {
                return NotFound(new { message = "No employee data found for the given parameters." });
            }

            return Ok(new { message = "Employee list data fetched successfully.", data = result });
        }


    }
}
