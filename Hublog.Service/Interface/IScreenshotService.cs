using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface IScreenshotService
    {
        Task<List<UserScreenShot>> GetUserScreenShots(int userId, int organizationId, DateTime date);
    }
}
