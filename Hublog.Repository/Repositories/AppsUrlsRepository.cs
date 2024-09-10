using Dapper;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Interface;
using System.Data;

namespace Hublog.Repository.Repositories
{
    public class AppsUrlsRepository : IAppsUrlsRepository
    {
        private readonly Dapperr _dapper;
        public AppsUrlsRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        #region TrackApplicationUsage
        public async Task TrackApplicationUsage(int userId, string applicationName, string totalUsage, string details, DateTime usageDate, string url)
        {
            if (!await UserExists(userId))
            {
                throw new Exception("User does not exist.");
            }

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                parameters.Add("@ApplicationName", applicationName);
                parameters.Add("@TotalUsage", totalUsage);
                parameters.Add("@Details", details);
                parameters.Add("@UsageDate", usageDate);
                parameters.Add("@Url", url);

                var result = await _dapper.ExecuteAsync("InsertOrUpdateApplicationUsage", parameters, commandType: CommandType.StoredProcedure);

                if (result <= 0)
                {
                    Console.WriteLine("Failed to log application usage.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error tracking application usage: {ex.Message}");
                throw; 
            }
        }

        private async Task<bool> UserExists(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var result = await _dapper.GetSingleAsync<int>("SELECT COUNT(1) FROM Users WHERE Id = @UserId", parameters);
            return result > 0;
        }

        #endregion

        #region GetUsersApplicationUsages
        public async Task<List<GetApplicationUsage>> GetUsersApplicationUsages(int organizationId, int userId, DateTime startDate, DateTime endDate)
        {
            string query = @"
                    SELECT 
                        au.Id AS ApplicationUsageId,
                        au.UserId,
                        u.Email,
                        u.OrganizationId,
                        au.ApplicationName,
                        au.TotalUsage,
                        au.Details,
                        au.CreatedDate,
                        au.UsageDate,
                        au.Url
                    FROM 
                        ApplicationUsage AS au
                    JOIN 
                        Users AS u ON au.UserId = u.Id
                    JOIN 
                        Organization AS o ON u.OrganizationId = o.Id
                    WHERE 
                        u.OrganizationId = @OrganizationId  
                        AND au.UserId = @UserId              
                        AND au.UsageDate BETWEEN @StartDate AND @EndDate";

            var parameter = new { organizationId, userId, startDate, endDate };

            return await _dapper.GetAllAsync<GetApplicationUsage>(query, parameter);
        }
        #endregion

    }
}
