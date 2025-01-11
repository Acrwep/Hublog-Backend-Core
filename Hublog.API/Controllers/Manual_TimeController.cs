using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model.Manual_Time;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Manual_TimeController : ControllerBase
    {
        private readonly IManual_TimeService _manual_TimeService;
        public Manual_TimeController(IManual_TimeService manual_TimeService)
        {
            _manual_TimeService = manual_TimeService;
        }
        [HttpPost("InsertManualTime")]
        public async Task<IActionResult> InsertManualTime()
        {
            try
            {
                var form = Request.Form;

                var datePart = form["date"];
                var startTimePart = form["startTime"];
                var endTimePart = form["endTime"];

                var parsedDate = DateTime.ParseExact(datePart, "yyyy-MM-dd HH:mm:ss.fff", null);

                var attachmentFile = Request.Form.Files.FirstOrDefault();
                byte[]? fileData = null;
                string? fileName = null;

                if (attachmentFile != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await attachmentFile.CopyToAsync(memoryStream);
                        fileData = memoryStream.ToArray();
                        fileName = attachmentFile.FileName;
                    }
                }

                var manualTime = new Manual_Time
                {
                    OrganizationId = int.Parse(form["OrganizationId"]),
                    UserId = int.Parse(form["UserId"]),
                    Date = parsedDate,

                    StartTime = DateTime.ParseExact($"{parsedDate:yyyy-MM-dd} {startTimePart}", "yyyy-MM-dd HH:mm:ss", null),
                    EndTime = DateTime.ParseExact($"{parsedDate:yyyy-MM-dd} {endTimePart}", "yyyy-MM-dd HH:mm:ss", null),

                    Summary = form["summary"],
                    AttachmentData = fileData,
                    FileName = fileName
                };

                var result = await _manual_TimeService.InsertManualTime(manualTime);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("get_Manual_Time")]
        public async Task<IActionResult> GetManualTime(int organizationId,int? teamid, int? userId)
        {
            try
            {
                var manualTimeEntries = await _manual_TimeService.GetManualTime(organizationId, teamid, userId);

                if (manualTimeEntries == null || !manualTimeEntries.Any())
                {
                    return NotFound(new { Message = "No manual time entries found." });
                }

                return Ok(manualTimeEntries);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


    }
}
