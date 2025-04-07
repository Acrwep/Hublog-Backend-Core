using Hublog.Repository.Entities.UpdatePassword;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly IForgotPasswordService _forgotPasswordService;

        public ForgotPasswordController(IForgotPasswordService forgotPasswordService)
        {
            _forgotPasswordService = forgotPasswordService;
        }

        [HttpGet]
        [Route("CheckEmailExists")]
        public async Task<IActionResult> CheckEmailExists([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Email is required." });
            }

            bool exists = await _forgotPasswordService.CheckEmailExistsAsync(email);

            if (exists)
            {
                return Ok(new { message = "Success! Email exists in the database." });
            }
            else
            {
                return NotFound(new { message = "Error! Your email is not available in the database." });
            }
        }

        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            var updateResult = await _forgotPasswordService.UpdatePasswordAsync(request.Email, request.NewPassword);

            if (updateResult.isUpdated)
            {
                return Ok(new { message = $"Your Password {request.NewPassword} updated successfully.", newUser = updateResult.newUser });
            }
            else
            {
                return NotFound(new { message = $"Error! Your Email {request.Email} not found in the database." });
            }
        }

        //[HttpPut("update-password")]
        //public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        //{
        //    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.NewPassword))
        //    {
        //        return BadRequest(new { message = "Email and password are required."});
        //    }

        //    bool isUpdated = await _forgotPasswordService.UpdatePasswordAsync(request.Email, request.NewPassword);

        //    if (isUpdated)
        //    {
        //        return Ok(new { message = $"Your Password {request.NewPassword} updated successfully."});
        //    }
        //    else
        //    {
        //        return NotFound(new { message = $"Error!Your Email {request.Email} not found in the database."});
        //    }
        //}
    }
}
