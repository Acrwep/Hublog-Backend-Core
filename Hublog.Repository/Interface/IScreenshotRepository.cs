
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.UserModels;

namespace Hublog.Repository.Interface
{
    public interface IScreenshotRepository
    {
        Task<List<UserScreenShot>> GetUserScreenShots(int userId, int organizationId, DateTime date);
    }
}
