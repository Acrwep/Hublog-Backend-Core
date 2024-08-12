using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController : ControllerBase
    {
        private readonly IDesignationService _designationService;
        public DesignationController(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        #region GetDesignationAll
        [HttpGet("GetDesignationAll")]
        public async Task<IActionResult> GetDesignationAll(int organizationId)
        {
            try
            {
                var result = await _designationService.GetDesignationAll(organizationId);
                if (result.Any())
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
        #endregion 


    }
}
