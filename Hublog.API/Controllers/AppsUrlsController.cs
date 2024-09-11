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

        #region GetApplicationUsage
        [HttpGet("GetAppUsage")]
        public async Task<IActionResult> GetApplicationUsage(int? userId,int? teamid, int organizationId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var usageData = await _appsUrlsService.GetUsersApplicationUsages(organizationId, teamid, userId, startDate, endDate);
                return Ok(usageData);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving usage data: {ex.Message}");
            }
        }
        #endregion

        [HttpGet("GetUrlUsage")]
        public async Task<IActionResult> GetUrlUsage(int? userId, int? teamid, int organizationId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var usageData = await _appsUrlsService.GetUsersUrlUsages(organizationId, teamid, userId, startDate, endDate);
                return Ok(usageData);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving usage data: {ex.Message}");
            }
        }


        [HttpPost("Application")]
        public async Task<IActionResult> LogApplicationUsage(ApplicationUsage applicationUsage)
        {
            var result = await _appsUrlsService.LogApplicationUsageAsync(applicationUsage);
            if (result)
                return Ok(new { Message = "Application usage logged successfully" });
            return BadRequest(new { Message = "Failed to log application usage" });
        }

        [HttpPost("Url")]
        public async Task<IActionResult> LogUrlUsage(UrlUsage urlUsage)
        {
            var result = await _appsUrlsService.LogUrlUsageAsync(urlUsage);
            if (result)
                return Ok(new { Message = "URL usage logged successfully" });
            return BadRequest(new { Message = "Failed to log URL usage" });
        }
    }
}
