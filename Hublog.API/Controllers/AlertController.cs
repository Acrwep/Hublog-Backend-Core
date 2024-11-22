using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
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
        public async Task<IActionResult> GetAlert(int organizationId, int? userId, DateTime triggeredTime = default)
        {
            if (triggeredTime == default)
            {
                return BadRequest("Invalid or missing triggered time.");
            }

            var result = await _alertService.GetAlert(organizationId, userId, triggeredTime);
            return Ok(result);
        }
    }
}
