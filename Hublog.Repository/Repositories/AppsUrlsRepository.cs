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
 SUM(
        -- Convert TotalUsage into total seconds manually
        CAST(SUBSTRING(au.TotalUsage, 1, CHARINDEX(':', au.TotalUsage) - 1) AS INT) * 3600 +  -- Hours to seconds
        CAST(SUBSTRING(au.TotalUsage, CHARINDEX(':', au.TotalUsage) + 1, CHARINDEX(':', au.TotalUsage, CHARINDEX(':', au.TotalUsage) + 1) - CHARINDEX(':', au.TotalUsage) - 1) AS INT) * 60 +  -- Minutes to seconds
        CAST(SUBSTRING(au.TotalUsage, CHARINDEX(':', au.TotalUsage, CHARINDEX(':', au.TotalUsage) + 1) + 1, LEN(au.TotalUsage)) AS INT)  -- Seconds
    ) AS TotalUsage
        FROM 
            ApplicationUsage AS au
        JOIN 
            Users AS u ON au.UserId = u.Id
        WHERE 
            u.OrganizationId = @OrganizationId  
            AND u.Active = 1
            AND (@UserId IS NULL OR au.UserId = @UserId)  
            AND (@TeamId IS NULL OR u.TeamId = @TeamId)  
            AND au.UsageDate BETWEEN @StartDate AND @EndDate
        GROUP BY 
            au.ApplicationName
        ORDER BY 
             au.ApplicationName;";

            var parameter = new { organizationId, teamid, userId, startDate, endDate };
            //return await _dapper.GetAllAsync<GetApplicationUsage>(query, parameter);
            var rawData = await _dapper.GetAllAsync<GetApplicationUsage>(query, parameter);

            foreach (var record in rawData)
            {
                record.TotalUsage = FormatDuration(long.Parse(record.TotalUsage)); 
            }

            return rawData;
        }
        private string FormatDuration(long totalSeconds)
        {
            var hours = totalSeconds / 3600; // Total hours
            var minutes = (totalSeconds % 3600) / 60; // Remaining minutes
            var seconds = totalSeconds % 60; // Remaining seconds
            return $"{hours:D2}:{minutes:D2}:{seconds:D2}"; // Format as "HH:mm:ss"
        }

        #endregion

        public async Task<List<GetUrlUsage>> GetUsersUrlUsages(int organizationId, int? teamid, int? userId, DateTime startDate, DateTime endDate)
        {
            string query = @"
SELECT 
	uu.Url,
    COUNT(DISTINCT uu.UserId) AS UserCount,  
 SUM(
        CAST(SUBSTRING( uu.TotalUsage, 1, CHARINDEX(':', uu.TotalUsage) - 1) AS INT) * 3600 +  -- Hours to seconds
        CAST(SUBSTRING( uu.TotalUsage, CHARINDEX(':',  uu.TotalUsage) + 1, CHARINDEX(':',  uu.TotalUsage, CHARINDEX(':', uu.TotalUsage) + 1) - CHARINDEX(':',  uu.TotalUsage) - 1) AS INT) * 60 +  -- Minutes to seconds
        CAST(SUBSTRING( uu.TotalUsage, CHARINDEX(':', uu.TotalUsage, CHARINDEX(':',  uu.TotalUsage) + 1) + 1, LEN( uu.TotalUsage)) AS INT)  -- Seconds
    ) AS TotalUsage 
FROM 
    UrlUsage AS uu
JOIN 
    Users AS u ON uu.UserId = u.Id
WHERE 
    u.OrganizationId = @OrganizationId  
    AND u.Active = 1
    AND (@UserId IS NULL OR uu.UserId = @UserId)  
    AND (@TeamId IS NULL OR u.TeamId = @TeamId)  
    AND uu.UsageDate BETWEEN @StartDate AND @EndDate
GROUP BY 
    uu.Url  
ORDER BY 
   uu.Url;";

            var parameter = new { organizationId, teamid, userId, startDate, endDate };

            //return await _dapper.GetAllAsync<GetUrlUsage>(query, parameter);
            var rawData = await _dapper.GetAllAsync<GetUrlUsage>(query, parameter);

            foreach (var record in rawData)
            {
                record.TotalUsage = FormatDuration(long.Parse(record.TotalUsage));
            }
            return rawData;
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
            var result = await _dapper.GetSingleAsync<(string Url, long MaxUsageSeconds)>(
                query,
                parameters,
                CommandType.StoredProcedure
            );

            string FormatDuration(long totalSeconds)
            {
                var hours = totalSeconds / 3600;
                var minutes = (totalSeconds % 3600) / 60;
                var seconds = totalSeconds % 60;
                return $"{hours}:{minutes:D2}:{seconds:D2}";
            }

            return (result.Url, FormatDuration(result.MaxUsageSeconds));
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

            // Ensure the field names match the stored procedure output
            var result = await _dapper.GetSingleAsync<(string ApplicationName, long TotalUsageSeconds)>(
                query,
                parameters,
                CommandType.StoredProcedure
            );

            string FormatDuration(long totalSeconds)
            {
                var hours = totalSeconds / 3600;        
                var minutes = (totalSeconds % 3600) / 60; 
                var seconds = totalSeconds % 60;        
                return $"{hours}:{minutes:D2}:{seconds:D2}"; 
            }

            return (result.ApplicationName, FormatDuration(result.TotalUsageSeconds));
        }

        public async Task<List<AppUsage>> GetAppUsages(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            string appUsageQuery = "get_ApplicationUsage";

            string urlUsageQuery = "Get_UrlUsage";
            ;
            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                FromDate = fromDate,
                ToDate = toDate
            };
            IEnumerable<AppUsage> appUsages;
            IEnumerable<AppUsage> urlUsages;

            appUsages = await _dapper.GetAllAsync<AppUsage>(appUsageQuery, parameters);
            urlUsages = await _dapper.GetAllAsync<AppUsage>(urlUsageQuery, parameters);

            var allUsages = appUsages.Concat(urlUsages).ToList();
            return allUsages;
        }
        public async Task<(string ApplicationName, string MaxUsage)> GetTopCategory(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            var teamQuery = @"
               SELECT T.Id
                FROM Team T
                INNER JOIN Organization O ON T.OrganizationId = O.Id
                WHERE O.Id = @OrganizationId
                AND (@TeamId IS NULL OR T.Id = @TeamId) ";

            var teams = await _dapper.GetAllAsync<int>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });

            var categoryTimeDictionary = new Dictionary<string, int>();

            foreach (var team in teams)
            {
                teamId = team;
                var urlUsageQuery = "Get_CombinedUsageData";
                var parameters = new
                {
                    OrganizationId = organizationId,
                    TeamId = teamId,
                    UserId = userId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                IEnumerable<App_UrlModel> usages = await _dapper.GetAllAsync<App_UrlModel>(urlUsageQuery, parameters);

                //var usages = await GetAppUsages(organizationId, teamId, userId, fromDate, toDate);

                var totalUsages = usages
                    .GroupBy(u => u.Name)
                   .Select(g => new { ApplicationName = g.Key, TotalSeconds = g.Sum(u => u.TotalSeconds) })
                   .ToDictionary(t => t.ApplicationName, t => t.TotalSeconds);

                foreach (var usage in usages)
                {
                    usage.Name = usage.Name.ToLower();

                    if (usage.Name != "chrome" && usage.Name != "msedge")
                    {
                        // Update usage time
                        if (totalUsages.TryGetValue(usage.Name, out var totalSeconds))
                        {
                            usage.TotalSeconds = totalSeconds;
                            usage.TotalUsage = TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");
                        }

                        var imbuildAppQuery = @"
    SELECT CategoryId 
    FROM ImbuildAppsAndUrls 
    WHERE Name LIKE '%' + @ApplicationName + '%'";
                        var categoryId = await _dapper.QueryFirstOrDefaultAsync<int?>(imbuildAppQuery, new { ApplicationName = usage.Name });

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
            string FormatDuration(double totalSeconds)
            {
                var totalHours = (long)(totalSeconds / 3600); // Total hours
                var minutes = (long)((totalSeconds % 3600) / 60); // Remaining minutes
                var seconds = (long)(totalSeconds % 60); // Remaining seconds
                return $"{totalHours:D2}:{minutes:D2}:{seconds:D2}"; // Format as "HH:mm:ss"
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
            return(mostUsedCategoryName, FormatDuration(mostUsedCategoryTime));
        }

    }

}