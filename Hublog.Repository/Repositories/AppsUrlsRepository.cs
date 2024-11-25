using Dapper;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.ApplicationModel;
using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Repository.Entities.Model.UrlModel;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        //public async Task<int> InsertApplicationUsageAsync(ApplicationUsage applicationUsage)
        //{
        //    return 0;
        //}


        //public async Task<int> InsertUrlUsageAsync(UrlUsage urlUsage)
        //{
        //    return 0;
        //}

        //public async Task<bool> LogApplicationUsage(ApplicationUsage applicationUsage)
        //{

        //    var query = "INSERT INTO ApplicationUsage (UserId, ApplicationName, TotalUsage, UsageDate, Details) VALUES (@UserId, @ApplicationName, @TotalUsage, @UsageDate, @Details);";

        //    var result = await _dapper.ExecuteAsync(query, applicationUsage);

        //    return result>0;

        //}
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

        public async Task<List<AppUsage>> GetAppUsages(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            string appUsageQuery = @"
           SELECT 
               A.UserId, 
               A.ApplicationName, 
               A.Details, 
               SUM(DATEDIFF(SECOND, '00:00:00', A.TotalUsage)) AS TotalSeconds, 
               A.UsageDate
           FROM  
               ApplicationUsage A
           INNER JOIN 
               Users U ON A.UserId = U.Id
           INNER JOIN 
               Organization O ON U.OrganizationId = O.Id
           WHERE  
                  O.Id = @OrganizationId 
                  AND A.UsageDate BETWEEN @FromDate AND @ToDate
                  AND (@TeamId IS NULL OR U.TeamId = @TeamId)
                  AND (@UserId IS NULL OR A.UserId = @UserId)
           GROUP BY 
               A.UserId, 
               A.ApplicationName, 
               A.Details, 
               A.UsageDate;
        ";

            var urlUsageQuery = @"
             SELECT U.UserId,
                 U.Url AS ApplicationName,
                 U.Details,
                 SUM(DATEDIFF(SECOND, '00:00:00', U.TotalUsage)) AS TotalSeconds,
                 U.UsageDate
             FROM UrlUsage U
             INNER JOIN 
               Users Us ON U.UserId = Us.Id
             INNER JOIN 
               Organization O ON Us.OrganizationId = O.Id
             WHERE 
               O.Id = @OrganizationId 
               AND U.UsageDate BETWEEN @FromDate AND @ToDate
               AND (@TeamId IS NULL OR US.TeamId = @TeamId)
               AND (@UserId IS NULL OR U.UserId = @UserId)
             GROUP BY 
               U.UserId,
               U.Url,
               U.Details, 
               U.UsageDate;"
            ;
            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                FromDate = fromDate,
                ToDate = toDate
            };

            var appUsages = await _dapper.GetAllAsync<AppUsage>(appUsageQuery, parameters);
            var urlUsages = await _dapper.GetAllAsync<AppUsage>(urlUsageQuery, parameters);

            var allUsages = appUsages.Concat(urlUsages).ToList();

            // Return the merged list return allUsages;
            return allUsages;
            //var urlUsageQuery1 = @"select * imImbuildAppsAndUrls where name Like '' ",;


        }
        public async Task<(string ApplicationName, string MaxUsage)> GetTopCategory(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            var teamQuery = @"
               SELECT T.Id
                FROM Team T
                INNER JOIN Organization O ON T.OrganizationId = O.Id
                WHERE O.Id = @OrganizationId
                AND (@TeamId IS NULL OR T.Id = @TeamId) ";

            // Fetching teams from the database
            var teams = await _dapper.GetAllAsync<int>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });

            // Dictionary to store usage time for each category across all teams
            var categoryTimeDictionary = new Dictionary<string, int>();

            // Process each team
            foreach (var team in teams)
            {
                teamId = team;
                var usages = await GetAppUsages(organizationId, teamId, userId, fromDate, toDate);

                // Process the usages and calculate total usage time for each application
                var totalUsages = usages
                    .GroupBy(u => u.ApplicationName)
                    .Select(g => new { ApplicationName = g.Key, TotalSeconds = g.Sum(u => u.TotalSeconds) })
                    .ToDictionary(t => t.ApplicationName, t => t.TotalSeconds);

                foreach (var usage in usages)
                {
                    usage.ApplicationName = usage.ApplicationName.ToLower();

                    // Skip certain browsers if needed
                    if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge" && usage.ApplicationName != "firefox" && usage.ApplicationName != "opera")
                    {
                        // Update usage time
                        if (totalUsages.TryGetValue(usage.ApplicationName, out var totalSeconds))
                        {
                            usage.TotalSeconds = totalSeconds;
                            usage.TotalUsage = TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");
                        }

                        // Query for category details
                        var imbuildAppQuery = @"
    SELECT CategoryId 
    FROM ImbuildAppsAndUrls 
    WHERE Name LIKE '%' + @ApplicationName + '%'";
                        var categoryId = await _dapper.QueryFirstOrDefaultAsync<int?>(imbuildAppQuery, new { ApplicationName = usage.ApplicationName });

                        if (categoryId.HasValue)
                        {
                            var categoryQuery = @"
    SELECT CategoryName 
    FROM Categories 
    WHERE Id = @CategoryId";
                            var categoryName = await _dapper.QueryFirstOrDefaultAsync<string>(categoryQuery, new { CategoryId = categoryId.Value });

                            if (!string.IsNullOrEmpty(categoryName))
                            {
                                // Accumulate usage time by category
                                if (!categoryTimeDictionary.ContainsKey(categoryName))
                                {
                                    categoryTimeDictionary[categoryName] = 0;
                                }

                                categoryTimeDictionary[categoryName] += usage.TotalSeconds;
                            }
                        }
                    }
                }
            }

            // Determine the most-used category after processing all teams
            string mostUsedCategoryName = null;
            int mostUsedCategoryTime = 0;

            if (categoryTimeDictionary.Any())
            {
                var mostUsedCategory = categoryTimeDictionary.OrderByDescending(kv => kv.Value).First();
                mostUsedCategoryName = mostUsedCategory.Key;
                mostUsedCategoryTime = mostUsedCategory.Value;
            }

            // Return the result after processing all teams
            return (mostUsedCategoryName, TimeSpan.FromSeconds(mostUsedCategoryTime).ToString(@"hh\:mm\:ss"));
        }

    }

}