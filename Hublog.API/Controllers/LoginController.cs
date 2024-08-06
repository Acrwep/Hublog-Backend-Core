using Hublog.Repository.Entities.Login;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        #region Login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModels model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model state is not valid");
            }

            try
            {
                var (user, token) = await _loginService.Login(model);
                if (user == null)
                {
                    return NotFound("Invalid username or password");
                }

                return Ok(new { user, token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region AdminLogin
        [AllowAnonymous]
        [HttpPost("AdminLogin")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginModels model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model State is Not Valid");
            }

            try
            {
                var (admin, token) = await _loginService.AdminLogin(model.UserName, model.Password);
                if(admin != null)
                {
                    return Ok(new { admin, token });
                }
                else
                {
                    return NotFound("Invalid UserName & Password");
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occured: " + ex.Message);
            }
        }
        #endregion

        #region UserLogin
        [AllowAnonymous]
        [HttpPost("Userlogin")]
        public async Task<IActionResult> UserLogin([FromBody] LoginModels model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model State is Not Valid");
            }

            try
            {
                var (user, token) = await _loginService.UserLogin(model.UserName, model.Password);

                if (user != null)
                {
                    return Ok(new { user, token });
                }
                else
                {
                    return NotFound("Invalid UserName & Password");
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
            }
        }
        #endregion
    }
}
