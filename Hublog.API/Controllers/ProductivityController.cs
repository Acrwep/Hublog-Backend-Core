using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
       // [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
        [HttpGet("Category")]
        public async Task<IActionResult> GetCategory(int organizationId,[FromQuery] string categoryName = "")
        {
            var categories = await _productivityService.GetCategoryProductivity(categoryName, organizationId);

            if (categories == null || !categories.Any())
                return NotFound(new { message = "No categories found." });

            return Ok(categories);
        }


        [HttpPost("InsertDefaultCategoryRecords/{organizationId}")]
        public async Task<IActionResult> InsertDefaultCategoryRecords(int organizationId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Please Give OrganizationId");
                }
                await _productivityService.InsertDefaultCategoryRecordsAsync(organizationId);
                return Ok(new { message = "Default Category records inserted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Default Category records insert data: {ex.Message}");
            }

        }


        [HttpDelete("DeleteImbuildAppOrUrl")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _productivityService.DeleteByIdAsync(id);
                if (result)
                {
                    return Ok(new { Message = "Imbuild AppsAndUrls deleted successfully" });
                }
                return BadRequest(new { Message = "Imbuild AppsAndUrls not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
        [HttpPut("UpdateProductivityId")]
        public async Task<IActionResult> UpdateProductivityId(int categoryId, int? productivityId)
        {
            var isUpdated = await _productivityService.UpdateProductivityId(categoryId, productivityId);

            if (isUpdated)
                return Ok(new { message = "ProductivityId updated successfully." });
            else
                return NotFound(new { message = "Category not found." });
        }
        [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
        [HttpGet("GetImbuildAppsAndUrls")]
        public async Task<IActionResult> GetImbuildAppsAndUrls(int OrganizationId,string userSearchQuery = "",string type="",string category="")
        {
            try
            {
                var categories = await _productivityService.GetImbuildAppsAndUrls(OrganizationId,userSearchQuery, type, category);
                if (categories != null && categories.Any())
                    return Ok(new { message = "ProductivityId updated successfully.", data = categories });
                else
                    return NotFound(new { message = "Categories not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
        [HttpGet("GetByIdImbuildAppsAndUrls")]
        public async Task<IActionResult> GetByIdImbuildAppsAndUrls(int id)
        {
            try
            {
                var categories = await _productivityService.GetByIdImbuildAppsAndUrls(id);

              if (categories != null) 
                  return Ok(categories);
              else
                  return NotFound(new { message = "Category not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
        [HttpPost("AddImbuildAppsAndUrls")]
        public async Task<IActionResult> AddImbuildAppsAndUrls(MappingModel mappingModel)
        {
            try
            {
                var result = await _productivityService.AddImbuildAppsAndUrls(mappingModel);

                return Ok(new { message = "Data inserted successfully", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("InsertDefaultAppsAndUrlsRecords/{organizationId}")]
        public async Task<IActionResult> InsertDefaultAppsAndUrlsRecords(int organizationId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Please Give OrganizationId");
                }
                await _productivityService.InsertDefaultRecordsAsync(organizationId);
                return Ok(new { message = "Default records inserted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Default records insert data: {ex.Message}");
            }

        }

       

        [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
        [HttpPut("InsertImbuildAppsAndUrls/{id}")]
        public async Task<IActionResult> InsertImbuildAppsAndUrls(int id, [FromBody] MappingModel model)
        {
            try
            {
                 var result = await _productivityService.InsertImbuildAppsAndUrls(id, model);

                 return Ok(new { message = "Data inserted successfully", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetProductivityBreakDown")]
        public async Task<IActionResult> GetProductivityDurations(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var result = await _productivityService.GetProductivityDurations(organizationId, teamId, userId, fromDate, toDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
        [HttpGet("Teamwise_Productivity")]
        public async Task<IActionResult> TeamwiseProductivity(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
           try
           { 
                var result = await _productivityService.TeamwiseProductivity(organizationId, teamId, fromDate, toDate);

                return Ok(result);
           }
           catch (Exception ex)
           {
                return BadRequest(ex.Message);
           }
        }
        [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
        [HttpGet("Most&Least_Teamwise_Productivity")]
        public async Task<IActionResult> MostTeamwiseProductivity(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                 var result = await _productivityService.MostTeamwiseProductivity(organizationId, teamId, fromDate, toDate);

                 return Ok(result);
            }
            catch (Exception ex)
            {
                 return BadRequest(ex.Message);
            }
        }

        [HttpGet("Total_Working_Time")]
        public async Task<IActionResult> GetTotal_Working_Time(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var result = await _productivityService.GetTotal_Working_Time(organizationId, teamId, userId, fromDate, toDate);

                return Ok(new { message = "Total working time data fetched successfully.", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
        [HttpGet("GetProductivity_Trend")]
        public async Task<IActionResult> GetProductivity_Trend(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
             try
             {

                  var result = await _productivityService.GetProductivity_Trend(organizationId, teamId, userId, fromDate, toDate);

                  return Ok(new { message = "Productivity trend data fetched successfully.", data = result });
             }
             catch (Exception ex)
             {
                 return BadRequest(ex.Message);
             }
        }

        [HttpGet("GetEmployeeList")]
        public async Task<IActionResult> GetEmployeeList(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
           try
           {
               var result = await _productivityService.GetEmployeeList(organizationId, teamId, userId, fromDate, toDate);

               return Ok(new { message = "Employee list data fetched successfully.", data = result });
           }
           catch (Exception ex)
           {
               return BadRequest(ex.Message);
           }
        }

        [HttpGet("category-usage-percentage")]
        public async Task<IActionResult> GetCategoryUsagePercentage(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var categories = await _productivityService.GetCategoryUsagePercentage(organizationId, teamId, userId, fromDate, toDate);

                return Ok(new { message = "Categories list data fetched successfully.", data = categories });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
