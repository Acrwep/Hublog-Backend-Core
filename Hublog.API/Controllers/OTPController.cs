using Hublog.Repository.Entities.Model.OTPRequest;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OTPController : ControllerBase
    {
       
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;

        public OTPController( IEmailService emailService, IOtpService otpService)
        {
            _emailService = emailService;
            _otpService = otpService;
        }
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequest otpRequest)
        {
            await _emailService.SendOtpAsync(otpRequest);
            return Ok(new { Message = "OTP sent successfully to your email." });
        }

        [HttpPost("validate-otp")]
        public async Task<IActionResult> ValidateOtp([FromBody] OtpValidationRequest request)
        {
            bool isValid = await _emailService.ValidateOTP(request);
            return isValid ? Ok(new { Message = "OTP validated successfully" }) : BadRequest(new { Message = "Invalid or expired OTP" });
        }
    }
}
