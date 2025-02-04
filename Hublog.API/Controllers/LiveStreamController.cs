using Hublog.API.Hub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Hublog.Repository.Entities.Model.LivestreamModal;
using System.Threading.Tasks;

namespace Hublog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiveStreamController : ControllerBase
    {
        private readonly IHubContext<LiveStreamHub> _hubContext;

        public LiveStreamController(IHubContext<LiveStreamHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("SendLiveData")]
        public async Task<IActionResult> SendLiveData([FromBody] LivestreamModal request)
        {
            //if (int.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.OrganizationId))
            //{
            //    return BadRequest("UserId and OrganizationId are required.");
            //}

            string groupName = $"org_{request.OrganizationId}_user_{request.UserId}";
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveLiveData", request.ActiveApp);

            return Ok("Live data sent successfully.");
        }
    }
}
