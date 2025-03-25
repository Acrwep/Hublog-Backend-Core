using Hublog.Repository.Common;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
       
        public UsersController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        #region InsertBreak
        [HttpPost("InsertBreak")]
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
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
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
        #endregion

        [HttpPost("PunchIn_InsertAttendance")]
        public async Task<IActionResult> PunchIn_InsertAttendance([FromBody]List<UserAttendanceModel> model)
        {
            try
            {
                var result= await  _userService.PunchIn_InsertAttendance(model);
                if (result==null)
                {
                    return BadRequest("Your shift time is not starting yet");
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost("PunchoutInsertAttendance")]
        public async Task<IActionResult> PunchoutInsertAttendance([FromBody] List<UserAttendanceModel> model)
        {

            try
            {
                await _userService.PunchoutInsertAttendance(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        #region UploadFile
        [HttpPost("UploadFile")]
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
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
        [Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
        public async Task<IActionResult> GetUserAttendanceDetails([FromQuery] int organizationId ,[FromQuery] int userId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var (records, summary) = await _userService.GetUserAttendanceDetails(organizationId, userId, startDate, endDate);

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
        [HttpGet("GetUserPunchInOutDetails")]
        public async Task<IActionResult> GetUserPunchInOutDetails(int userId,int organizationId, DateTime startDate, DateTime endDate)
        {
            var updatedUser = await _userService.GetUserPunchInOutDetails(userId, organizationId, startDate, endDate);

            return Ok(updatedUser);
        }
        [HttpPut("UpdateUserAttendanceDetails")]
        public async Task<IActionResult> UpdateUserAttendanceDetails([FromBody] AttendanceUpdate request)
        {
                var updatedUser = await _userService.UpdateUserAttendanceDetails(request);

                    return Ok(updatedUser);
        }

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
        [Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
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
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
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
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
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
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
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
        [Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
        public async Task<IActionResult> GetAllUsers(int organizationid, string searchQuery = "")
        {
            try
            {
                var claimsPrincipal = User as ClaimsPrincipal;
                var loggedInUserEmail = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                var result = await _userService.GetAllUser(loggedInUserEmail, organizationid, searchQuery);

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

        [HttpGet("GetActiveUsers")]
        [Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
        public async Task<IActionResult> GetActiveUsers(int organizationid, string searchQuery = "")
        {
            try
            {
                var claimsPrincipal = User as ClaimsPrincipal;
                var loggedInUserEmail = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                var result = await _userService.GetActiveUsers(loggedInUserEmail, organizationid, searchQuery);

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
                    await _emailService.SendEmailAsync(user);

                    if (createdUser == null)
                    {
                        return Conflict("A user with this email already exists");
                    }

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
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // Return custom error message
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating user");
            }
        }

        [HttpDelete("DeleteUser/{id}")]
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

        [HttpGet("GetTotalBreak")]
        [Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
        public async Task<IActionResult> GetUserTotalBreak(int organizationId, int userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = await _userService.GetUserTotalBreak(organizationId, userId, startDate, endDate);
                if(result != null)
                {
                    return Ok(result);
                }
                return NotFound("No data found");
            }
            catch(Exception ex) 
            {
                return BadRequest("An error occured while processing your request");
            }
        }
        [HttpPost("Insert_Active_Time")]
        public async Task<IActionResult> Insert_Active_Time(UserActivity activity)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var createdBreakMaster = await _userService.Insert_Active_Time(activity);
                    if (createdBreakMaster != null)
                    {
                        return CreatedAtAction(nameof(Insert_Active_Time), new { id = createdBreakMaster.Id }, createdBreakMaster);
                    }
                    else
                    {
                        return StatusCode(500, "Could not create Breakmaster");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("Model state is not valid");
            }
        }
        [HttpGet("Get_Active_Time")]
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
        public async Task<IActionResult> Get_Active_Time(int userid, DateTime startDate, DateTime endDate)
        {
            try
            {
                var breakMasters = await _userService.Get_Active_Time(userid, startDate, endDate);
                return Ok(breakMasters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }
        [HttpPost("Insert_IdealActivity")]
        //[Authorize(Policy = CommonConstant.Policies.UserOrAdminPolicy)]
        public async Task<IActionResult> Insert_IdealActivity(IdealActivity activity)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var createdBreakMaster = await _userService.Insert_IdealActivity(activity);
                    if (createdBreakMaster != null)
                    {
                        return CreatedAtAction(nameof(Insert_IdealActivity), new { id = createdBreakMaster.Id }, createdBreakMaster);
                    }
                    else
                    {
                        return StatusCode(500, "Could not create Breakmaster");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("Model state is not valid");
            }
        }

        [HttpGet]
        [Route("GetPunchIn_Users")]
        public async Task<IActionResult> GetPunchInUser(int organizationId,[FromQuery] DateTime date)
        {
            try
            {
                var result = await _userService.GetPunchIn_Users(organizationId,date);
                if (result ==null || !result.Any())
                {
                    return BadRequest("PunchIn Users Not Found");
                }
              
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
