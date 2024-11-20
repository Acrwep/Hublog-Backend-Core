using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Service.Interface;
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
        public async Task<IActionResult> InsertAlert([FromBody] Alert alert)
        {
            if (alert == null)
            {
                return BadRequest("Alert information cannot be null.");
            }

            var result = await _alertService.InsertAlert(alert);

            if (result)
            {
                return Ok("Alert processed successfully.");
            }

            return NotFound("Alert could not be processed.");
        }

        [HttpGet("GetAlert")]
        public async Task<IActionResult> GetAlert(int id, int userId, string triggered = "", DateTime triggeredTime = default)
        {
            if (triggeredTime == default)
            {
                return BadRequest("Invalid or missing triggered time.");
            }

            var result = await _alertService.GetAlert(id, userId, triggered, triggeredTime);
            return Ok(result);
        }
    }
}
