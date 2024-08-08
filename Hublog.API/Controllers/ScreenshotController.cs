using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenshotController : ControllerBase
    {
        private readonly IScreenshotService _screenshotService;
        private readonly ILogger<ScreenshotController> _logger;
        public ScreenshotController(IScreenshotService screenshotService, ILogger<ScreenshotController> logger)
        {
            _screenshotService = screenshotService;
            _logger = logger;
        }

        #region GetUserScreenShots
        [HttpGet("GetUserScreenShots")]
        public async Task<IActionResult> GetUserScreenShots(int userId, int organizationId, DateTime date)
        {
            try
            {
                var userScreenshotResult = await _screenshotService.GetUserScreenShots(userId, organizationId, date);
                return Ok(userScreenshotResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user.");
                return StatusCode(500, "Internal server error.");
            }
        }
        #endregion
    }
}
