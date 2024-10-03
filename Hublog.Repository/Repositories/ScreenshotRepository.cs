using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;

namespace Hublog.Repository.Repositories
{
    public class ScreenshotRepository : IScreenshotRepository
    {
        private readonly Dapperr _dapper;
        public ScreenshotRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }
        public async Task<List<UserScreenShot>> GetUserScreenShots(int userId, int organizationId, DateTime date)
        {
            // Use Yours
            return null;
        }
    }
}
