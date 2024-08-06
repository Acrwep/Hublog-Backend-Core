using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface IScreenshotService
    {
        Task<List<UserScreenShotModels>> GetUserScreenShots(int userId, int organizationId, DateTime date);
    }
}
