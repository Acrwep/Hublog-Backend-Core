using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.ApplicationModel;
using Hublog.Repository.Entities.Model.UrlModel;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
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
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
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
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
        public async Task<IActionResult> LogApplicationUsage(ApplicationUsage applicationUsage)
        {
            var result = await _appsUrlsService.LogApplicationUsageAsync(applicationUsage);
            if (result)
                return Ok(new { Message = "Application usage logged successfully" });
            return BadRequest(new { Message = "Failed to log application usage" });
        }

        [HttpPost("Url")]
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
        public async Task<IActionResult> LogUrlUsage(UrlUsage urlUsage)
        {
            var result = await _appsUrlsService.LogUrlUsageAsync(urlUsage);
            if (result)
                return Ok(new { Message = "URL usage logged successfully" });
            return BadRequest(new { Message = "Failed to log URL usage" });
        }

        [HttpGet("GetTopUrlUsage")]
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
        public async Task<IActionResult> GetTopUrlUsage(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = await _appsUrlsService.GetTopUrlUsageAsync(organizationId, teamId, userId, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving top URL usage: {ex.Message}");
            }
        }

        [HttpGet("GetTopAppUsage")]
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
        public async Task<IActionResult> GetTopAppUsage(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = await _appsUrlsService.GetTopAppUsageAsync(organizationId, teamId, userId, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving top URL usage: {ex.Message}");
            }
        }
    }
}
