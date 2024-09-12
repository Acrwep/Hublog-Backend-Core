using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
    }
}
