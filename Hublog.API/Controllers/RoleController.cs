using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        #region  GetRoleByOrganizationId
        [HttpGet("GetRoleByOrganizationId")]
        public async Task<IActionResult> GetRoleByOrganizationId(int organizationId)
        {
            try
            {
                var result = await _roleService.GetRoleByOrganizationId(organizationId);
                if(result.Any())
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound("No data found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region GetAllRole
        [HttpGet("GetRoleAll")]
        public async Task<IActionResult> GetRoleAll()
        {
            try
            {
                var result = await _roleService.GetRoleAll();
                if (result.Any())
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound("No data found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        #endregion

        #region InsertRole
        [HttpPost("InsertRole")]
        public async Task<IActionResult> InsertRole(Role role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Modal state is invalid");
            }
            try
            {
                var result = await _roleService.InsertRole(role);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the designation.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region UpdateRole
        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateRole(Role role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Modal state is not valid");
            }
            try
            {
                var result = await _roleService.UpdateRole(role);
                if(result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound("no data found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating Role");
            }
        }
        #endregion

        #region DeleteRole
        [HttpDelete("DeleteRole")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            try
            {
                bool isDeleted = await _roleService.DeleteRole(roleId);
                if (isDeleted)
                {
                    return Ok($"Designation with {roleId} is deleted");
                }
                else
                {
                    return NotFound($"Designation with {roleId} not found");
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting Role");
            }
        }
        #endregion
    }
}
