using Microsoft.AspNetCore.Http;

namespace Hublog.Repository.Entities.DTO
{
    public class UserScreenshotDTO
    {
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public string ScreenShotType { get; set; }
        public DateTime ScreenShotDate { get; set; }
        public IFormFile File { get; set; }
    }
}
