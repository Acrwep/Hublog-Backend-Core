using Hublog.Repository.Entities.Model;
using Hublog.Service.Interface;
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
        public async Task<IActionResult> GetSystemInfo(int organizationId, int? teamId = null, string userSearchQuery = "", string platformSearchQuery = "", string systemTypeSearchQuery = "")
        {
            try
            {
                var result = await _systemInfoService.GetSystemInfo(organizationId, teamId, userSearchQuery, platformSearchQuery, systemTypeSearchQuery);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
