using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        #region InsertBreak
        [HttpPost("InsertBreak")]
        public async Task<IActionResult> InsertBreak(List<UserBreakModel> model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model State is Not Valid");
            }

            try
            {
                var result = await _userService.InsertBreak(model);

                if (result.Result == 1)
                {
                    return Ok("InsertBreak Success");
                }
                else
                {
                    return NotFound($"Error: {result.Msg}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
        #endregion

        #region InsertAttendance
        [HttpPost("InsertAttendance")]
        public async Task<IActionResult> InsertAttendance(List<UserAttendanceModel> model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _userService.InsertAttendance(model);
                return Ok("InsertAttendance Success");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
        #endregion

        #region UploadFile
        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                var headers = Request.Headers;
                if (!headers.ContainsKey("UId") || !headers.ContainsKey("OId") || !headers.ContainsKey("SType") || !headers.ContainsKey("SDate"))
                {
                    return BadRequest("Missing required headers.");
                }

                var userScreenshotDTO = new UserScreenshotDTO   
                {
                    UserId = int.Parse(headers["UId"]),
                    OrganizationId = int.Parse(headers["OId"]),
                    ScreenShotType = headers["SType"],
                    ScreenShotDate = DateTime.Parse(headers["SDate"]),
                    File = Request.Form.Files.FirstOrDefault()
                };

                await _userService.SaveUserScreenShot(userScreenshotDTO);
                return Ok("Upload successful.");
            }
            catch (FormatException ex)
            {
                return BadRequest($"Header parsing error: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion

        #region GetUserAttendanceDetails
        [HttpGet("GetUserAttendanceDetails")]
        public async Task<IActionResult> GetUserAttendanceDetails([FromQuery] int userId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var (records, summary) = await _userService.GetUserAttendanceDetails(userId, startDate, endDate);

                var responseModel = new
                {
                    AttendanceDetails = records,
                    AttendanceSummary = summary
                };

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        #endregion

        #region GetUsersByTeamId
        [HttpGet("GetUsersByTeamId")]
        public async Task<IActionResult> GetUsersByTeamId([FromQuery] int teamId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model state is not valid");
            }

            try
            {
                var result = await _userService.GetUsersByTeamId(teamId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region GetUsersByOrganizationId
        [HttpGet("GetUsersByOrganizationId")]
        public async Task<IActionResult> GetUsersByOrganizationId(int organizationId)
        {
            try
            {
                var result = await _userService.GetUsersByOrganizationId(organizationId);
                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound("No data found");
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while processing your request");
            }
        }
        #endregion 
    }
}
