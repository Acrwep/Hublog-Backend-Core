using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;
        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }
        [HttpGet("GetActivityBreakDown")]
        public async Task<IActionResult> GetActivityBreakDown(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var result = await _activityService.GetActivityBreakDown(organizationId, teamId, userId, fromDate, toDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("Date_wise_Activity")]
        public async Task<IActionResult>Date_wise_Activity(int organizationId, int? teamId,int? userid,[FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var result = await _activityService.Date_wise_Activity(organizationId, teamId, userid, fromDate, toDate);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetActivityEmployeeList")]
        public async Task<IActionResult> GetActivityEmployeeList(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var result = await _activityService.GetActivityEmployeeList(organizationId, teamId, userId, fromDate, toDate);

                return Ok(new { message = "Employee list data fetched successfully.", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
