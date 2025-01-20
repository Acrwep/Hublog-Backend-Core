using Hublog.Repository.Common;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = CommonConstant.Policies.AdminPolicy)]
    public class AttendanceDashboardController : ControllerBase
    {
        private readonly IAttendanceDashboardService _attendanceDashboardService;
        public AttendanceDashboardController(IAttendanceDashboardService attendanceDashboardService)
        {
            _attendanceDashboardService = attendanceDashboardService;
        }

        [HttpGet("AttendanaceAndBreakSummary")]
        public async Task<IActionResult> GetUserTotalAttendanceAndBreakSummary([FromQuery] int organizationId,
                                                                         [FromQuery] int? teamId,
                                                                         [FromQuery] int? userId,
                                                                         [FromQuery] DateTime startDate,
                                                                         [FromQuery] DateTime endDate)
        {
            var result = await _attendanceDashboardService.GetUserTotalAttendanceAndBreakSummary(organizationId, teamId, userId, startDate, endDate);   
            return Ok(result);
        }

        [HttpGet("AllAttendanceSummary")]
        public async Task<IActionResult> GetAllAttendanceSummary([FromQuery] int organizationId,
                                                                         [FromQuery] int? teamId,
                                                                         [FromQuery] int? userId,
                                                                         [FromQuery] DateTime startDate,
                                                                         [FromQuery] DateTime endDate)
        {
            var result = await _attendanceDashboardService.GetAllAttendanceSummary(organizationId,teamId, userId, startDate, endDate);
            return Ok(result);
        }

        [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetAttendanceDashboardSummary([FromQuery] int organizationId, [FromQuery] int? teamId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _attendanceDashboardService.AttendanceDashboardSummary(organizationId, teamId, startDate, endDate);
            return Ok(result);
        }
        
        [HttpGet("BreakTrends")]
        public async Task<IActionResult> BreakTrends([FromQuery] int organizationId, [FromQuery] int? teamId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _attendanceDashboardService.BreakTrends(organizationId, teamId, startDate, endDate);
            return Ok(result);
        }



        [HttpGet("top-productivity-Teams")]
        public async Task<IActionResult> GetTopTeamProductivity(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            var result = await _attendanceDashboardService.GetTopTeamProductivity(organizationId, teamId, startDate, endDate);
            if (result == null || result.Count == 0)
            {
                return NotFound("No data found");
            }
            return Ok(result);
        }

        [HttpGet("Least-Productivity-Teams")]
        public async Task<IActionResult> GetLeastTeamProductivity(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            var result = await _attendanceDashboardService.GetLeastTeamProductivity(organizationId,teamId, startDate, endDate);
            if(result == null || result.Count == 0)
            {
                return NotFound("No data found");
            }
            return Ok(result);
        }

        [HttpGet("LateArrivals")]
        public async Task<IActionResult> GetLateArrivals(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            var result = await _attendanceDashboardService.GetLateArrivals(organizationId, teamId, startDate, endDate);
            if(result == null ){
                return NotFound("No data found");
            }
            return Ok(result);
        }
    }
}
