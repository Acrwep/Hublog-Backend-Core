using Dapper;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.ApplicationModel;
using Hublog.Repository.Entities.Model.UrlModel;
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

        #region GetUsersApplicationUsages
        public async Task<List<GetApplicationUsage>> GetUsersApplicationUsages(int organizationId, int? teamid, int? userId, DateTime startDate, DateTime endDate)
        {
            string query = @"
        SELECT 
            au.ApplicationName,
            COUNT(DISTINCT au.UserId) AS UserCount,
            CONVERT(VARCHAR(8), DATEADD(SECOND, SUM(DATEDIFF(SECOND, 0, TRY_CAST(au.TotalUsage AS TIME))), 0), 108) AS TotalUsage,
            au.UsageDate
        FROM 
            ApplicationUsage AS au
        JOIN 
            Users AS u ON au.UserId = u.Id
        WHERE 
            u.OrganizationId = @OrganizationId  
            AND (@UserId IS NULL OR au.UserId = @UserId)  
            AND (@TeamId IS NULL OR u.TeamId = @TeamId)  
            AND au.UsageDate BETWEEN @StartDate AND @EndDate
        GROUP BY 
            au.ApplicationName, au.UsageDate  
        ORDER BY 
            au.UsageDate;";

            var parameter = new { organizationId, teamid, userId, startDate, endDate };

            return await _dapper.GetAllAsync<GetApplicationUsage>(query, parameter);
        }

        #endregion

        public async Task<List<GetUrlUsage>> GetUsersUrlUsages(int organizationId, int? teamid, int? userId, DateTime startDate, DateTime endDate)
        {
            string query = @"
SELECT 
	uu.Url,
    COUNT(DISTINCT uu.UserId) AS UserCount,  
    CONVERT(VARCHAR(8), DATEADD(SECOND, SUM(
        DATEDIFF(SECOND, 0, TRY_CAST(uu.TotalUsage AS TIME))
    ), 0), 108) AS TotalUsage,
    uu.UsageDate
FROM 
    UrlUsage AS uu
JOIN 
    Users AS u ON uu.UserId = u.Id
WHERE 
    u.OrganizationId = @OrganizationId  
    AND (@UserId IS NULL OR uu.UserId = @UserId)  
    AND (@TeamId IS NULL OR u.TeamId = @TeamId)  
    AND uu.UsageDate BETWEEN @StartDate AND @EndDate
GROUP BY 
    uu.Url, uu.UsageDate  
ORDER BY 
    uu.UsageDate;";

            var parameter = new { organizationId, teamid, userId, startDate, endDate };

            return await _dapper.GetAllAsync<GetUrlUsage>(query, parameter);
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

        public async Task<(string Url, string MaxUsage)> GetTopUrlUsageAsync(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate)
        {
            var parameters = new
            {
                @OrganizationId = organizationId,
                @TeamId = teamId,
                @UserId = userId,
                @StartDate = startDate,
                @EndDate = endDate
            };

            string query = "GetTopUrlUsage";

            return await _dapper.GetSingleAsync<(string Url, string MaxUsage)>(query, parameters, CommandType.StoredProcedure);
        }

        public async Task<(string ApplicationName, string MaxUsage)> GetTopAppUsageAsync(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate)
        {
            var parameters = new
            {
                @OrganizationId = organizationId,
                @TeamId = teamId,
                @UserId = userId,
                @StartDate = startDate,
                @EndDate = endDate
            };

            string query = "GetTopAppUsage";

            return await _dapper.GetSingleAsync<(string Url, string MaxUsage)>(query, parameters, CommandType.StoredProcedure);
        }
    }
}
