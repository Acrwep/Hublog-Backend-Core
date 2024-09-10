using Hublog.Repository.Entities.Model;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppsUrlsController : ControllerBase
    {
        private readonly IAppsUrlsService _appsUrlsService;
        public AppsUrlsController(IAppsUrlsService appsUrlsService)
        {
            _appsUrlsService = appsUrlsService;
        }

        #region TrackApplicationUsage
        [HttpPost("AppUsage")]
        public async Task<IActionResult> TrackApplicationUsage([FromBody] ApplicationUsage usageDto)
        {
            if (usageDto == null)
            {
                return BadRequest("Usage data is null.");
            }

            try
            {
                await _appsUrlsService.TrackApplicationUsage(usageDto.UserId, usageDto.ApplicationName, usageDto.TotalUsage, usageDto.Details, usageDto.UsageDate, usageDto.Url);
                return Ok("Usage data logged successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging usage data: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion

        #region GetApplicationUsage
        [HttpGet("GetAppUsage")]
        public async Task<IActionResult> GetApplicationUsage(int userId, int organizationId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var usageData = await _appsUrlsService.GetUsersApplicationUsages(organizationId, userId, startDate, endDate);
                return Ok(usageData);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving usage data: {ex.Message}");
            }
        }
        #endregion
    }
}
