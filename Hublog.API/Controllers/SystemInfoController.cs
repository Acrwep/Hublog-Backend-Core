using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemInfoController : ControllerBase
    {
        private readonly ISystemInfoService _systemInfoService;
        public SystemInfoController(ISystemInfoService systemInfoService)   
        {
            _systemInfoService = systemInfoService;
        }

        [HttpPost("InsertOrUpdateSystemInfo")]
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
        public async Task<IActionResult> InsertOrUpdateSystemInfo([FromBody] SystemInfoModel systemInfoModel)
        {
            if (systemInfoModel == null)
            {
                return BadRequest("System information cannot be null.");
            }

            var result = await _systemInfoService.InsertOrUpdateSystemInfo(systemInfoModel);

            if (result)
            {
                return Ok("System info processed successfully.");
            }

            return NotFound("System info could not be processed.");
        }

        [HttpGet("GetSystemInfo")]
        [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
        public async Task<IActionResult> GetSystemInfo(int organizationId, int? userid, int? teamId = null, string userSearchQuery = "", string platformSearchQuery = "", string systemTypeSearchQuery = "")
        {
            try
            {
                var result = await _systemInfoService.GetSystemInfo(organizationId,userid, teamId, userSearchQuery, platformSearchQuery, systemTypeSearchQuery);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetSystemInfoCount")]
        [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
        public async Task<IActionResult> GetSystemInfoCount(int organizationId, int? teamId = null, int? userId = null, string userSearchQuery = "", string platformSearchQuery = "", string systemTypeSearchQuery = "")
        {
            var result = await _systemInfoService.GetSystemInfoCount(organizationId, teamId, userId, userSearchQuery, platformSearchQuery, systemTypeSearchQuery);
            return Ok(result);
        }
    }
}
