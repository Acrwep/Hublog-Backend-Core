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

        #region BreakReport
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
        #endregion

        #region GetMonthlyAttendanceReport
        [HttpGet("GetMonthlyAttendanceReport")]
        public async Task<IActionResult> GetMonthlyAttendanceReport(int? userId, int? teamId, int organizationId, int year, int month)
        {
            var attendanceLogs = await _reportService.GetMonthlyAttendanceReport(userId, teamId, organizationId, year, month);

            if (attendanceLogs == null || !attendanceLogs.Any())
            {
                return NotFound("No attendance records found for the specified criteria.");
            }

            var response = new
            {
                OrganizationId = organizationId,
                users = attendanceLogs
                    .GroupBy(x => new { x.UserId, x.First_Name, x.Last_Name })
                    .Select(g => new
                    {
                        //userId = g.Key.UserId,
                        first_name = g.Key.First_Name,
                        last_name = g.Key.Last_Name,
                        logs = g.Select(r => new
                        {
                            date = r.AttendanceDate.ToString("yyyy-MM-dd"),
                            total_time = r.Total_Time,
                            day_status = r.DayStatus
                        }).ToList()
                    }).ToList()
            };

            return Ok(response);
        }
        #endregion
    }
}
