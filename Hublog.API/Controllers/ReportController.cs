using Hublog.Repository.Common;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
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
                return NotFound("No attendance records found.");
            }

            var response = new
            {
                OrganizationId = organizationId,
                users = attendanceLogs
                    .GroupBy(x => new { x.UserId, x.Full_Name,x.Team_Name })
                    .Select(g => new
                    {
                        Full_Name = g.Key.Full_Name,
                        Team_Name = g.Key.Team_Name,
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

        #region GetMonthlyInOutReport
        [HttpGet("GetMonthlyInOutReport")]
        public async Task<IActionResult> GetMonthlyInOutReport(int? userId, int? teamId, int organizationId, int year, int month)
        {
            var inOutLogs = await _reportService.GetMonthlyInOutReport(userId, teamId, organizationId, year, month);
            if (inOutLogs == null || !inOutLogs.Any())
            {
                return NotFound("No IN and OUT records found.");
            }
            var response = new
            {
                OrganizationId = organizationId,
                users = inOutLogs
                        .GroupBy(u => new { u.Full_Name, u.Team_Name })
                        .Select(s => new
                        {
                            Full_Name = s.Key.Full_Name,
                            Team_Name=s.Key.Team_Name,
                            logs = s.Select(ss => new
                            {
                                date = ss.AttendanceDate.ToString("yyyy-MM-dd"),
                                In = ss.Start_Time,
                                Out = ss.End_Time,
                                day_status = ss.DayStatus,
                            }).ToList()
                        }).ToList(),
            };

            return Ok(response);
        }
        #endregion

        [HttpGet("combined")]
        public async Task<IActionResult> GetCombinedUsage(int organizationId, int? teamId, int? userId, string type, DateTime startDate, DateTime endDate)
        {
            var result = await _reportService.GetCombinedUsageReport(organizationId, teamId, userId, type, startDate, endDate);
            return Ok(result);
        }
    }
}
