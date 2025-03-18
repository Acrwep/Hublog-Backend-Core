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
    }
}
