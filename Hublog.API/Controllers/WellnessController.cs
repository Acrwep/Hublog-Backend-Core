﻿using Hublog.Repository.Common;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.Organization;
using Hublog.Repository.Entities.Model.WellNess_Model;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WellnessController : ControllerBase
    {
        private readonly IWellnessService _WellnessService;
        public WellnessController(IWellnessService wellnessService)
        {
            _WellnessService = wellnessService;
        }
       
        #region GetUsersByTeamId
        [HttpGet("GetWellness")]
        public async Task<IActionResult> GetWellness([FromQuery] int OrganizationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model state is not valid");
            }

            try
            {
                var result = await _WellnessService.GetWellness(OrganizationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region InsertWellness
        [HttpPost("insertWellness")]
        public async Task<IActionResult> Insert([FromBody] WellNess wellNess)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Model state is not valid");
                }

                var insertedRecord = await _WellnessService.InsertWellnessAsync(wellNess);

                if (insertedRecord != null)
                {
                    return Ok(insertedRecord);
                }
                return BadRequest("Insert Wellness failed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        #endregion


        [HttpPut("UpdateWellNess")]
        public async Task<IActionResult> UpdateWellNess(int OrganizationId, WellNess WellNess)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var updatedBreakMaster = await _WellnessService.UpdateWellNess(OrganizationId, WellNess);
                    if (updatedBreakMaster != null)
                    {
                        return Ok(updatedBreakMaster);
                    }
                    else
                    {
                        return NotFound("AlertRule Not Found");
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error updating AlertRule");
                }
            }
            else
            {
                return BadRequest("Model state is not valid");
            }
        }
        [HttpGet("GetWellnessSummary")]
        public async Task<IActionResult> GetWellnessSummary(int organizationId, int? teamId, [FromQuery] DateTime Date)
        {
            try
            {
                var result = await _WellnessService.GetWellnessSummary(organizationId, teamId,Date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetWellnessTimeTrend")]
        public async Task<IActionResult> GetWellnessDetails(int organizationId, int? teamId,int? userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var result = await _WellnessService.GetWellnessDetails(organizationId, teamId, userId, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetWellnessUserDetails")]
        public async Task<IActionResult> GetWellnessUserDetails(int organizationId, int? teamId, int? userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var result = await _WellnessService.GetWellnessUserDetails(organizationId, teamId, userId, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
