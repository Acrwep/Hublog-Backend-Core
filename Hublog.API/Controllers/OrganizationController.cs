using Hublog.Repository.Entities.Model.Organization;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpPost("insert")]
        public async Task<IActionResult> Insert([FromBody] Organizations organization)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Model state is not valid");
                }

                var insertedRecord = await _organizationService.InsertAsync(organization);

                if (insertedRecord != null)
                {
                    return CreatedAtAction(nameof(Insert), new { id = insertedRecord.Id }, insertedRecord);
                }
                return BadRequest("Insert failed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] Organizations organization)
        {
            try
            {
                var updatedOrganization = await _organizationService.UpdateAsync(organization);
                if (updatedOrganization != null)
                {
                    return Ok(updatedOrganization);
                }
                else
                {
                    return NotFound("Organization Not Found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _organizationService.GetAllAsync();
            return Ok(result);
        }

    }
}
