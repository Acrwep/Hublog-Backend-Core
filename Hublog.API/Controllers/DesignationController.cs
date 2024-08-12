using Hublog.Repository.Entities.Model;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
    public class DesignationController : ControllerBase
    {
        private readonly IDesignationService _designationService;
        private readonly ILogger<DesignationController> _logger;
        public DesignationController(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        #region GetDesignationAll
        [HttpGet("GetDesignationAll")]
        public async Task<IActionResult> GetDesignationAll(int organizationId)
        {
            try
            {
                var result = await _designationService.GetDesignationAll(organizationId);
                if (result.Any())
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
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region GetDesignationById
        [HttpGet("GetDesignationById")]
        public async Task<IActionResult> GetDesignationById(int organizationId, int designationId)    
        {
            try
            {
                var result = await _designationService.GetDesignationById(organizationId, designationId);
                if(result != null)
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

        #region InsertDesignation
        [HttpPost("InsertDesignation")]
        public async Task<IActionResult> InsertDesignation(Designation designation)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is not valid.");
                return BadRequest("Model State is Not Valid");
            }

            try
            {
                var createdDesignation = await _designationService.InsertDesignation(designation);
                return Ok(createdDesignation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the designation.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region UpdateDesignation
        [HttpPut("UpdateDesignation")]
        public async Task<IActionResult> UpdateDesignation(Designation designation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("ModelState is not valid");
            }

            try
            {
                var result = await _designationService.UpdateDesignation(designation);
                if(result != null)
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating designation");
            }
        }
        #endregion

        #region DeleteDesignation
        [HttpDelete("DeleteDesignation")]
        public async Task<IActionResult> DeleteDesignation(int organizationId, int designationId)
        {
            try
            {
                bool isDeleted = await _designationService.DeleteDesignation(organizationId, designationId);
                if (isDeleted)
                {
                    return Ok($"Designation with {designationId} is deleted");
                }
                else
                {
                    return NotFound($"Designation with {designationId} not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting designation");
            }
        }
        #endregion
    }
}
