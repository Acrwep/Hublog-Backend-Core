using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;
        private readonly ILogger<AdminController> _logger;
        public AlertController(IAlertService alertService, ILogger<AdminController> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        [HttpPost("InsertAlert")]
        public async Task<IActionResult> InsertAlert(List<Alert> alert)
        {
            if (alert == null)
            {
                return BadRequest("Alert information cannot be null.");
            }

            await _alertService.InsertAlert(alert);
            return Ok("InsertAttendance Success");

            return NotFound("Alert could not be processed.");
        }

        [HttpGet("GetAlert")]
        [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
        public async Task<IActionResult> GetAlert(int organizationId, int? teamId, int? userId, DateTime triggeredTime = default)
        {
            if (triggeredTime == default)
            {
                return BadRequest("Invalid or missing triggered time.");
            }

            var result = await _alertService.GetAlert(organizationId, teamId, userId, triggeredTime);
            return Ok(result);
        }
        [HttpPost("InsertAlertRule")]
        public async Task<IActionResult> InsertAlertRule(Alert_Rule alert_Rule)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var createdBreakMaster = await _alertService.InsertAlertRule(alert_Rule);
                    if (createdBreakMaster != null)
                    {
                        return CreatedAtAction(nameof(InsertAlertRule), new { id = createdBreakMaster.Id }, createdBreakMaster);
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
        [HttpPut("UpdateAlertRule")]
        [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
        public async Task<IActionResult> UpdateAlertRule(Alert_Rule alert_Rule)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var updatedBreakMaster = await _alertService.UpdateAlertRule(alert_Rule);
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
        [HttpGet("GetAlertRule")]
        public async Task<IActionResult> GetAlertRule(int organizationId, string? seachQuery = "")
        {
            try
            {
                var breakMasters = await _alertService.GetAlertRule(organizationId,seachQuery);
                return Ok(breakMasters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}