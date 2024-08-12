using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;   
        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;  
            _logger = logger;
        }

        #region UpdateBreakMaster
        [HttpPut("UpdateBreakMaster")]
        public async Task<IActionResult> UpdateBreakMaster(BreakMaster breakMaster)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var updatedBreakMaster = await _adminService.UpdateBreakMaster(breakMaster);
                    if(updatedBreakMaster != null)
                    {
                        return Ok(updatedBreakMaster);
                    }
                    else
                    {
                        return NotFound("BreakMaster Not Found");
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error updating breakMaster");
                }
            }
            else
            {
                return BadRequest("Model state is not valid");
            }
        }
        #endregion

        #region InsertBreakMaster
        [HttpPost("InsertBreakMaster")]
        public async Task<IActionResult> InsertBreakMaster(BreakMaster breakMaster)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var createdBreakMaster = await _adminService.InsertBreakMaster(breakMaster);
                    if(createdBreakMaster != null)
                    {
                        return CreatedAtAction(nameof(InsertBreakMaster), new { id = createdBreakMaster.Id }, createdBreakMaster);
                    }
                    else
                    {
                        return StatusCode(500, "Could not create Breakmaster");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("Model state is not valid");
            }
        }
        #endregion

        #region GetBreakMaster
        [HttpGet("GetBreakMaster")]
        public async Task<IActionResult> GetBreakMaster()
        {
            try
            {
                var breakMasters = await _adminService.GetBreakMasters();
                return Ok(breakMasters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user.");
                return StatusCode(500, "Internal server error.");
            }
        }
        #endregion

        #region GetAllUser
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(int organizationId)
        {
            try
            {
                var claimsPrincipal = User as ClaimsPrincipal;
                var loggedInUserEmail = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                var result = await _adminService.GetAllUser(loggedInUserEmail, organizationId);

                if (result != null && result.Any())
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound("No Data Found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllUsers");
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
