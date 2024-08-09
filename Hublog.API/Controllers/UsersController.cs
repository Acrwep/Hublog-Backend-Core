using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        #region GetAvailableBreak
        [HttpPost("GetAvailableBreak")]
        public async Task<IActionResult> GetAvailableBreak([FromBody] GetModels model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model State is Not Valid");
            }

            try
            {
                var result = await _userService.GetAvailableBreak(model);

                if (result.Count > 0)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound("No Data Found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region GetBreakMasterById
        [HttpGet("GetBreakMasterById")]
        public async Task<IActionResult> GetBreakMasterById(int id)
        {
            try
            {
                var result = await _userService.GetBreakMasterById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("error fetching data"); ;
            }
        }
        #endregion

        #region GetUserBreakRecordDetails
        [HttpGet("GetUserBreakRecordDetails")]
        public async Task<IActionResult> GetUserBreakRecordDetails([FromQuery] int userId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var result = await _userService.GetUserBreakRecordDetails(userId, startDate, endDate);
                if (result.Count > 0)
                {
                    return Ok(result);
                }
                return NotFound(new List<UserBreakRecordModel>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region  User CRUD Operation
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var claimsPrincipal = User as ClaimsPrincipal;
                var loggedInUserEmail = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                var result = await _userService.GetAllUser(loggedInUserEmail);

                if (result != null && result.Any())
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound("No Data Found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("InsertUser")]
        public async Task<IActionResult> InsertUser([FromBody] Users user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var createdUser = await _userService.InsertUser(user);
                    return CreatedAtAction(nameof(InsertUser), new { id = createdUser.Id }, createdUser);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                return BadRequest("Model State is Not Valid");
            }
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(Users user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model State is Not Valid");
            }

            try
            {
                var updatedUser = await _userService.UpdateUser(user);

                if (updatedUser != null)
                {
                    return Ok(updatedUser);
                }
                else
                {
                    return NotFound("User not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating user");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                bool isDeleted = await _userService.DeleteUser(id);

                if (isDeleted)
                {
                    return Ok($"User with Id {id} deleted successfully");
                }
                else
                {
                    return NotFound($"User with Id {id} not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting user");
            }
        }
        #endregion
    }
}
