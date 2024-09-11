using Dapper;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using System.Data;
using System.Data.Common;

namespace Hublog.Repository.Repositories
{
    public class AppsUrlsRepository : IAppsUrlsRepository
    {
        private readonly Dapperr _dapper;
        public AppsUrlsRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        #region GetUsersApplicationUsages
        public async Task<List<GetApplicationUsage>> GetUsersApplicationUsages(int organizationId,int? teamid, int? userId, DateTime startDate, DateTime endDate)
        {
            string query = @"
                    SELECT 
    au.Id AS ApplicationUsageId,
    au.UserId,
    u.Email,
    u.TeamId,
    u.OrganizationId,
    au.ApplicationName,
    au.TotalUsage,
    au.Details,
    au.UsageDate
FROM 
    ApplicationUsage AS au
JOIN 
    Users AS u ON au.UserId = u.Id
JOIN 
    Organization AS o ON u.OrganizationId = o.Id
WHERE 
    u.OrganizationId = @OrganizationId  
    AND (@UserId IS NULL OR au.UserId = @UserId)  
    AND (@TeamId IS NULL OR u.TeamId = @TeamId)  
    AND au.UsageDate BETWEEN @StartDate AND @EndDate;";

            var parameter = new { organizationId, teamid, userId, startDate, endDate };

            return await _dapper.GetAllAsync<GetApplicationUsage>(query, parameter);
        }
        #endregion

        public async Task<List<GetApplicationUsage>> GetUsersUrlUsages(int organizationId, int? teamid, int? userId, DateTime startDate, DateTime endDate)
        {
            string query = @"
SELECT 
    uu.Id AS UrlUsageId,
    uu.UserId,
    u.Email,
    u.TeamId,
    u.OrganizationId,
    uu.Url,
    uu.TotalUsage,
    uu.Details,
    uu.UsageDate
FROM 
    UrlUsage AS uu
JOIN 
    Users AS u ON uu.UserId = u.Id
JOIN 
    Organization AS o ON u.OrganizationId = o.Id
WHERE 
    u.OrganizationId = @OrganizationId  
    AND (@UserId IS NULL OR uu.UserId = @UserId)  
    AND (@TeamId IS NULL OR u.TeamId = @TeamId)  
    AND uu.UsageDate BETWEEN @StartDate AND @EndDate";

            var parameter = new { organizationId, teamid, userId, startDate, endDate };

            return await _dapper.GetAllAsync<GetApplicationUsage>(query, parameter);
        }

        public async Task<int> InsertApplicationUsageAsync(ApplicationUsage applicationUsage)
        {
            var sql = "EXEC InsertApplicationUsage @UserId, @ApplicationName, @TotalUsage, @UsageDate, @Details";
            return await _dapper.ExecuteAsync(sql, new
            {
                applicationUsage.UserId,
                applicationUsage.ApplicationName,
                applicationUsage.TotalUsage,
                applicationUsage.UsageDate,
                applicationUsage.Details
            });
        }

        public async Task<int> InsertUrlUsageAsync(UrlUsage urlUsage)
        {
            var sql = "EXEC InsertUrlUsage @UserId, @Url, @TotalUsage, @UsageDate, @Details";
            return await _dapper.ExecuteAsync(sql, new
            {
                urlUsage.UserId,
                urlUsage.Url,
                urlUsage.TotalUsage,
                urlUsage.UsageDate,
                urlUsage.Details
            });
        }

    }
}
