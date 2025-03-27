using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.ApplicationModel;
using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Repository.Entities.Model.UrlModel;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
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

        private bool FinalApplicationNameValidation(string applicationName)
        {
            // Remove all non-alphabetic characters and count only alphabetic letters
            var letterCount = applicationName.Count(char.IsLetter);

            // Check if there are at least two alphabetic letters
            return letterCount >= 2;
        }

        [HttpPost("Application")]
       // [Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
        public async Task<IActionResult> LogApplicationUsage(ApplicationUsage applicationUsage)
        {
            if (!ModelState.IsValid ||
                 string.IsNullOrWhiteSpace(applicationUsage.ApplicationName) ||
                 string.IsNullOrWhiteSpace(applicationUsage.TotalUsage) ||
                 string.IsNullOrWhiteSpace(applicationUsage.Details) ||
                 string.IsNullOrWhiteSpace(applicationUsage.UsageDate))
            {
                return BadRequest("Invalid Insert Operation: Fields cannot be empty.");
            }
            if (applicationUsage.ApplicationName == "firefox" || applicationUsage.ApplicationName == "msedge" || applicationUsage.ApplicationName == "chrome" ||  applicationUsage.ApplicationName == "opera" || applicationUsage.ApplicationName == "brave")
            {
                return BadRequest("Invalid Insert Operation");
            }

            bool finalValidationStatus = FinalApplicationNameValidation(applicationUsage.ApplicationName);
            if (finalValidationStatus==true)
            {
                var result = await _appsUrlsService.LogApplicationUsageAsync(applicationUsage);

                Console.WriteLine($"Result from InsertApplicationUsage: {result}");

                if (result)
                    return Ok(new { Message = "Application usage logged successfully" });
                else
                {
                    return BadRequest(new { Message = "Failed to log application usage" });
                }
            }
            else
            {
                return BadRequest("ApplicationName must have 2 Letters");
            }

           
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


        [HttpGet("GetTopAppAndUrlUsage")]
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
        public async Task<IActionResult> GetTopAppAndUrlUsage(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = await _appsUrlsService.GetTopAppAndUrlsUsageAsync(organizationId, teamId, userId, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving top URL usage: {ex.Message}");
            }
        }


        [HttpGet("GetTopCategory")]
        public async Task<IActionResult> GetTopCategory(int organizationId, int? teamId, int? userId,DateTime fromDate,  DateTime toDate)
        {
            var result = await _appsUrlsService.GetTopCategory(organizationId, teamId, userId, fromDate, toDate);
            return Ok(result);

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
                await _appsUrlsService.InsertDefaultRecordsAsync(organizationId);
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
    }
}
