using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class ScreenshotService : IScreenshotService
    {
        private readonly IScreenshotRepository _screenshotRepository;
        public ScreenshotService(IScreenshotRepository screenshotRepository)
        {
            _screenshotRepository = screenshotRepository;
        }

        public async Task<List<UserScreenShot>> GetUserScreenShots(int userId, int organizationId, DateTime date)
        {
            return await _screenshotRepository.GetUserScreenShots(userId, organizationId, date);
        }
    }
}
