using Hublog.Repository.Entities.Model.UserModels;
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

        public async Task<List<UserScreenShotDTO>> GetUserScreenShots(int userId, int organizationId, DateTime date)
        {
            return await _screenshotRepository.GetUserScreenShots(userId, organizationId, date);
        }
    }
}
