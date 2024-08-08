using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
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
            var query = @"
            SELECT uss.[Id]
                  ,uss.[UserId]
                  ,uss.[OrganizationId]
                  ,uss.[ScreenShotDate]
                  ,uss.[FileName]
                  ,uss.[ImageData]
                  ,u.[First_Name]
                  ,u.[Last_Name]
                  ,u.[Email]
              FROM [dbo].[UserScreenShots] uss
              JOIN [dbo].[Users] u ON uss.[UserId] = u.[Id]
              JOIN [dbo].[Team] t ON u.[TeamId] = t.[Id]
              WHERE CAST(uss.[ScreenShotDate] AS DATE) = @Date
                AND uss.[UserId] = @UserId
                AND uss.[OrganizationId] = @OrganizationId";

            return await _dapper.GetAllAsync<UserScreenShot>(query, new
            {
                UserId = userId,
                OrganizationId = organizationId,
                Date = date
            });
        }
    }
}
