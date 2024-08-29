using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;
        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        #region AttendanceReport
        [HttpGet("AttendanceReport")]
        public async Task<IActionResult> AttendanceReport(int? userId, int? teamId, int organizationId, DateTime date)
        {
            try
            {
                var attendanceReport = await _reportService.AttendanceReport(userId, teamId, organizationId, date);
                return Ok(attendanceReport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user.");
                return StatusCode(500, "Internal server error.");
            }
        }
        #endregion

        [HttpGet("BreakReport")]
        public async Task<IActionResult> BreakReport(int? userId, int? teamId, int organizationId, DateTime date)
        {
            try
            {
                var breakReport = await _reportService.BreakReport(userId, teamId, organizationId, date);
                return Ok(breakReport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
