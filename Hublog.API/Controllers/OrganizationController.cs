using Hublog.Repository.Entities.Model.Organization;
using Hublog.Repository.Entities.Model.OTPRequest;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using Hublog.Repository.Repositories;
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
        private readonly IEmailService _emailService;
       

        public OrganizationController(IOrganizationService organizationService, IEmailService emailService)
        {
            _organizationService = organizationService;
            _emailService = emailService;
           
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
                await _emailService.SendOrganizationEmailAsync(organization);

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


        [HttpGet("CheckDomain")]
        public async Task<IActionResult> CheckDomainAvailability([FromQuery] string domain)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(domain))
                {
                    return BadRequest("Domain name is required.");
                }

                bool domainExists = await _organizationService.CheckDomainAvailabilityAsync(domain);

                if (domainExists==false)
                {
                    return BadRequest(new { message = "The domain name is not exist." });
                }

                return Ok(new { message = "The domain name is exist." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

        [HttpGet("check-plan")]
        public async Task<IActionResult> CheckPlan(int organizationId)
        {
            try
            {
                var planStatus = await _organizationService.CheckOrganizationPlanAsync(organizationId);

                if (planStatus == "active")
                {
                    return Ok("Organization plan is active");
                }
                else if (planStatus == "expiring_soon")
                {
                    return Ok(new { message = "Organization plan is expiring soon. Please renew to avoid interruption." });
                }
                else if (planStatus == "expired")
                {
                    return BadRequest(new { error = "Organization plan is expired" });
                }
                else
                {
                    return BadRequest(new { error = "Unknown plan status" });
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


    }
}
