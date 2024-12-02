using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Repository.Entities.Model.UrlModel;
using Hublog.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Dynamic;

namespace Hublog.Repository.Repositories
{
    public class ProductivityRepository : IProductivityRepository
    {
        private readonly Dapperr _dapper;
        public ProductivityRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }
        public async Task<List<CategoryModel>> GetCategoryProductivity(string categoryName)
        {
            var query = @"
            SELECT 
                C.Id AS CategoryId,
                C.CategoryName,
                PA.Id AS ProductivityId,
                PA.Name AS ProductivityName
            FROM 
                Categories C
            LEFT JOIN 
                ProductivityAssign PA ON C.ProductivityId = PA.Id";

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                query += " WHERE C.CategoryName LIKE @CategoryName";
                var parameters = new { CategoryName = $"%{categoryName}%" };
                return await _dapper.GetAllAsync<CategoryModel>(query, parameters);
            }

            return await _dapper.GetAllAsync<CategoryModel>(query);
        }

        public async Task<int> UpdateProductivityId(int categoryId, int? productivityId)
        {
            var query = @"
            UPDATE Categories
            SET ProductivityId = @ProductivityId
            WHERE Id = @CategoryId";

            var parameters = new { CategoryId = categoryId, ProductivityId = productivityId };

            return await _dapper.ExecuteAsync(query, parameters);
        }
        public async Task<List<MappingModel>> GetImbuildAppsAndUrls()
        {
            //var query = "SELECT I.Type, I.Name " +
            //            "FROM ImbuildAppsAndUrls I " +
            //            "LEFT JOIN Categories C ON C.ProductivityId = I.CategoryId";

            // You can include the filtering logic here
            var query = "Select * From ImbuildAppsAndUrls";
            return await _dapper.GetAllAsync<MappingModel>(query);
        }
        public async Task<List<MappingModel>> GetByIdImbuildAppsAndUrls(int id)
        {
            var query = @"
        SELECT [id], [type], [name], [categoryid]
        FROM [EMP6].[dbo].[ImbuildAppsAndUrls] 
        WHERE [id] = @Id";

            var parameters = new { Id = id };
            var result = await _dapper.GetAllAsync<MappingModel>(query, parameters);

            return result;
        }

        public async Task<bool> InsertImbuildAppsAndUrls(int id, MappingModel model)
        {
            var query = @"
                UPDATE [EMP4].[dbo].[ImbuildAppsAndUrls]
                SET [CategoryId] = @NewCategoryId
                WHERE [id] = @Id";

            var parameters = new { Id = id, NewCategoryId = model.CategoryId };
            var affectedRows = await _dapper.ExecuteAsync(query, parameters);

            return affectedRows > 0;
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
                   Team T ON T.Id = U.TeamId
           INNER JOIN 
               Organization O ON U.OrganizationId = O.Id
           WHERE  
                  O.Id = @OrganizationId 
                  AND A.UsageDate BETWEEN @FromDate AND @ToDate
                  AND (@TeamId IS NULL OR T.Id = @TeamId)
                  AND (@UserId IS NULL OR A.UserId = @UserId)
           GROUP BY 
               A.UserId, 
               A.ApplicationName, 
               A.Details, 
               A.UsageDate;
        ";

            var urlUsageQuery = @"
    SELECT 
        U.UserId,
        U.Url AS ApplicationName,
        NULL AS Details,
        SUM(DATEDIFF(SECOND, '00:00:00', U.TotalUsage)) AS TotalSeconds,
        U.UsageDate
    FROM 
        UrlUsage U
    INNER JOIN 
        Users Us ON U.UserId = Us.Id
    INNER JOIN 
        Team T ON T.Id = Us.TeamId
    INNER JOIN 
        Organization O ON Us.OrganizationId = O.Id
    WHERE 
        O.Id = @OrganizationId 
        AND U.UsageDate BETWEEN @FromDate AND @ToDate
        AND (@TeamId IS NULL OR T.Id = @TeamId)
        AND (@UserId IS NULL OR U.UserId = @UserId)
    GROUP BY 
        U.UserId,
        U.Url,
        U.UsageDate;
";
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

            var allUsages = appUsages.Concat(urlUsages);

            // Group by ApplicationName (URL or Application) to handle duplicates
            var groupedUsages = allUsages
                .GroupBy(u => u.ApplicationName.ToLower()) // Grouping by URL in lowercase for case-insensitivity
                .Select(g => new AppUsage
                {
                    UserId = g.First().UserId, // You can choose how to handle UserId if there are multiple
                    ApplicationName = g.Key, // Use the grouped ApplicationName
                    Details = g.First().Details, // Keep the first Details (adjust as needed)
                    TotalSeconds = g.Sum(u => u.TotalSeconds), // Sum the TotalSeconds for the same URL
                    UsageDate = g.Min(u => u.UsageDate) // Optional: Use earliest date as the representative date
                })
                .ToList();

            // Return the merged and grouped list
            return groupedUsages;


        }
        public async Task<ProductivityDurations> GetProductivityDurations(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            var teamQuery = @"
                         SELECT T.Id
                          FROM Team T
                          INNER JOIN Organization O ON T.OrganizationId = O.Id
                          WHERE O.Id = @OrganizationId
                          AND (@TeamId IS NULL OR T.Id = @TeamId) ";

            // Fetching teams using the proper method and data type
            var teams = await _dapper.GetAllAsync<int>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });

            int totalProductiveDuration = 0;
            int totalUnproductiveDuration = 0;
            int totalNeutralDuration = 0;


            ProductivityDurations result = null;
            foreach (var team in teams)
            {
                teamId = team;
                var usages = await GetAppUsages(organizationId, teamId, userId, fromDate, toDate);


                // Calculate TotalUsage for each ApplicationName
                var totalUsages = usages
                .GroupBy(u => u.ApplicationName)
                .Select(g => new { ApplicationName = g.Key, TotalSeconds = g.Sum(u => u.TotalSeconds) })
                .ToDictionary(t => t.ApplicationName, t => t.TotalSeconds);

                // Initialize duration variables



                // Check against ImbuildAppsAndUrls and Categories tables
                foreach (var usage in usages)
                {
                    usage.ApplicationName = usage.ApplicationName.ToLower();

                    if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge" && usage.ApplicationName != "firefox" && usage.ApplicationName != "opera")
                    {
                        if (totalUsages.TryGetValue(usage.ApplicationName, out var totalSeconds))
                        {
                            usage.TotalSeconds = totalSeconds;
                            usage.TotalUsage = TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");
                        }

                        // Query for category and productivity details
                        var imbuildAppQuery = @"
                        SELECT CategoryId 
                         FROM ImbuildAppsAndUrls 
                        WHERE Name LIKE '%' + @ApplicationName + '%'";
                        var categoryId = await _dapper.QueryFirstOrDefaultAsync<int?>(imbuildAppQuery, new { ApplicationName = usage.ApplicationName });

                        if (categoryId.HasValue)
                        {
                            usage.CategoryId = categoryId.Value;

                            var categoryQuery = @"
                        SELECT CategoryName, ProductivityId 
                        FROM Categories 
                         WHERE Id = @CategoryId";

                            var category = await _dapper.QueryFirstOrDefaultAsync<(string CategoryName, int ProductivityId)>(categoryQuery, new { CategoryId = categoryId.Value });

                            if (category != default)
                            {
                                usage.CategoryName = category.CategoryName;

                                // Fetch ProductivityName from ProductivityAssign
                                var productivityQuery = @"
                            SELECT Name FROM ProductivityAssign
                            WHERE Id = @ProductivityId";

                                var productivityName = await _dapper.QueryFirstOrDefaultAsync<string>(productivityQuery, new { ProductivityId = category.ProductivityId });
                                usage.ProductivityName = productivityName;

                                // Add to the corresponding duration
                                switch (usage.ProductivityName)
                                {
                                    case "Productive":
                                        totalProductiveDuration += usage.TotalSeconds;
                                        break;
                                    case "Unproductive":
                                        totalUnproductiveDuration += usage.TotalSeconds;
                                        break;
                                    case "Neutral":
                                        totalNeutralDuration += usage.TotalSeconds;
                                        break;
                                }
                            }
                        }
                        //else
                        //{
                        //    // Assign to Neutral if no category is found
                        //    totalNeutralDuration += usage.TotalSeconds;
                        //}
                    }
                }

                // Create the result object
                var totalDurationInSeconds = totalProductiveDuration + totalUnproductiveDuration + totalNeutralDuration;

                var dateDifferenceInDays = (toDate - fromDate).TotalDays;

                if (dateDifferenceInDays <= 0)
                {
                    dateDifferenceInDays = 1;
                }
                var averageDurationInSeconds = totalProductiveDuration / dateDifferenceInDays;


                result = new ProductivityDurations
                {
                    TotalProductiveDuration = TimeSpan.FromSeconds(totalProductiveDuration).ToString(@"hh\:mm\:ss"),
                    TotalUnproductiveDuration = TimeSpan.FromSeconds(totalUnproductiveDuration).ToString(@"hh\:mm\:ss"),
                    TotalNeutralDuration = TimeSpan.FromSeconds(totalNeutralDuration).ToString(@"hh\:mm\:ss"),
                    TotalDuration = TimeSpan.FromSeconds(totalDurationInSeconds).ToString(@"hh\:mm\:ss"),
                    AverageDuratiopn = TimeSpan.FromSeconds(averageDurationInSeconds).ToString(@"hh\:mm\:ss")
                };

            }
            return result;

        }

        public async Task<List<AppUsage>> GetAppUsages(int organizationId, int? teamId, DateTime fromDate, DateTime toDate)
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
                   Users Us ON A.UserId = Us.Id
               INNER JOIN 
                   Team U ON us.TeamId = U.Id
               INNER JOIN 
                     Organization O ON U.OrganizationId = O.Id
               WHERE  
                  O.Id = @OrganizationId 
                  AND A.UsageDate BETWEEN @FromDate AND @ToDate
                  AND (@TeamId IS NULL OR U.Id = @TeamId)
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
              FROM  
                 Urlusage U
             INNER JOIN 
                   Users Us ON U.UserId = Us.Id
             INNER JOIN 
                   Team T ON us.TeamId = T.Id
             INNER JOIN 
                   Organization O ON T.OrganizationId = O.Id
             WHERE 
               O.Id = @OrganizationId 
               AND U.UsageDate BETWEEN @FromDate AND @ToDate
               AND (@TeamId IS NULL OR T.Id = @TeamId)
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
        public async Task<List<TeamProductivity>> TeamwiseProductivity(int organizationId, int? teamId, DateTime fromDate, DateTime toDate)
        {
            var teamQuery = @"
        SELECT T.Id, T.Name 
        FROM Team T
        INNER JOIN Organization O ON T.OrganizationId = O.Id
        WHERE O.Id = @OrganizationId
              AND (@TeamId IS NULL OR T.Id = @TeamId) ";

            var teams = await _dapper.GetAllAsync<(int TeamId, string TeamName)>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });

            var result = new List<TeamProductivity>();

            foreach (var team in teams)
            {
                teamId = team.TeamId;
                var usages = await GetAppUsages(organizationId, teamId, fromDate, toDate);

                var totalProductiveDuration = 0;
                var totalUnproductiveDuration = 0;
                var totalNeutralDuration = 0;

                foreach (var usage in usages)
                {
                    usage.ApplicationName = usage.ApplicationName.ToLower();

                    if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge" && usage.ApplicationName != "firefox" && usage.ApplicationName != "opera")
                    {
                        var imbuildAppQuery = @"
                    SELECT CategoryId 
                    FROM ImbuildAppsAndUrls 
                    WHERE Name LIKE '%' + @ApplicationName + '%'";

                        var categoryId = await _dapper.QueryFirstOrDefaultAsync<int?>(imbuildAppQuery, new { ApplicationName = usage.ApplicationName });

                        if (categoryId.HasValue)
                        {
                            var categoryQuery = @"
                    SELECT CategoryName, ProductivityId 
                    FROM Categories 
                    WHERE Id = @CategoryId";

                            var category = await _dapper.QueryFirstOrDefaultAsync<(string CategoryName, int ProductivityId)>(categoryQuery, new { CategoryId = categoryId.Value });

                            if (category != default)
                            {
                                var productivityQuery = @"
                        SELECT Name 
                        FROM ProductivityAssign 
                        WHERE Id = @ProductivityId";

                                var productivityName = await _dapper.QueryFirstOrDefaultAsync<string>(productivityQuery, new { ProductivityId = category.ProductivityId });

                                switch (productivityName)
                                {
                                    case "Productive":
                                        totalProductiveDuration += usage.TotalSeconds;
                                        break;
                                    case "Unproductive":
                                        totalUnproductiveDuration += usage.TotalSeconds;
                                        break;
                                    case "Neutral":
                                        totalNeutralDuration += usage.TotalSeconds;
                                        break;
                                }
                            }
                        }
                    }
                }

                var totalDurationInSeconds = totalProductiveDuration + totalUnproductiveDuration + totalNeutralDuration;

                var dateDifferenceInDays = (toDate - fromDate).TotalDays;
                if (dateDifferenceInDays <= 0)
                {
                    dateDifferenceInDays = 1;
                }

                var averageDurationInSeconds = totalProductiveDuration / dateDifferenceInDays;

                result.Add(new TeamProductivity
                {
                    TeamName = team.TeamName,
                    TotalProductiveDuration = TimeSpan.FromSeconds(totalProductiveDuration).ToString(@"hh\:mm\:ss"),
                    TotalNeutralDuration = TimeSpan.FromSeconds(totalNeutralDuration).ToString(@"hh\:mm\:ss"),
                    TotalUnproductiveDuration = TimeSpan.FromSeconds(totalUnproductiveDuration).ToString(@"hh\:mm\:ss"),
                    TotalDuration = TimeSpan.FromSeconds(totalDurationInSeconds).ToString(@"hh\:mm\:ss"),

                });
            }

            return result;
        }
        public async Task<dynamic> MostTeamwiseProductivity(int organizationId, int? teamId, DateTime fromDate, DateTime toDate)
        {
            var teamQuery = @"
        SELECT T.Id, T.Name 
        FROM Team T
        INNER JOIN Organization O ON T.OrganizationId = O.Id
        WHERE O.Id = @OrganizationId
        AND (@TeamId IS NULL OR T.Id = @TeamId)";

            var teams = await _dapper.GetAllAsync<(int TeamId, string TeamName)>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });

            if (!teams.Any())
            {
                return new
                {
                    GrandTotalpercentage = 0.0,
                    data = new
                    {
                        top = new[]
                        {
                    new
                    {
                        productive_duration = 0,
                        unproductive_duration = 0,
                        neutral_duration = 0,
                        total_duration = 0,
                        productive_percent = 0.0,
                        team_name = "N/A"
                    }
                },
                        bottom = new[]
                        {
                    new
                    {
                        productive_duration = 0,
                        unproductive_duration = 0,
                        neutral_duration = 0,
                        total_duration = 0,
                        productive_percent = 0.0,
                        team_name = "N/A"
                    }
                }
                    }
                };
            }

            var teamResults = new List<dynamic>();
            var GrandtotalTimeDuration = 0;
            var GrandtotalProductiveDuration = 0;
            foreach (var team in teams)
            {
                teamId = team.TeamId;
                var usages = await GetAppUsages(organizationId, teamId, fromDate, toDate);

                var totalProductiveDuration = 0;
                var totalUnproductiveDuration = 0;
                var totalNeutralDuration = 0;

                

                foreach (var usage in usages)
                {
                    usage.ApplicationName = usage.ApplicationName.ToLower();

                    if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge" && usage.ApplicationName != "firefox" && usage.ApplicationName != "opera")
                    {
                        var imbuildAppQuery = @"
                    SELECT CategoryId 
                    FROM ImbuildAppsAndUrls 
                    WHERE Name LIKE '%' + @ApplicationName + '%'";

                        var categoryId = await _dapper.QueryFirstOrDefaultAsync<int?>(imbuildAppQuery, new { ApplicationName = usage.ApplicationName });

                        if (categoryId.HasValue)
                        {
                            var categoryQuery = @"
                        SELECT CategoryName, ProductivityId 
                        FROM Categories 
                        WHERE Id = @CategoryId";

                            var category = await _dapper.QueryFirstOrDefaultAsync<(string CategoryName, int ProductivityId)>(categoryQuery, new { CategoryId = categoryId.Value });

                            if (category != default)
                            {
                                var productivityQuery = @"
                            SELECT Name 
                            FROM ProductivityAssign 
                            WHERE Id = @ProductivityId";

                                var productivityName = await _dapper.QueryFirstOrDefaultAsync<string>(productivityQuery, new { ProductivityId = category.ProductivityId });

                                switch (productivityName)
                                {
                                    case "Productive":
                                        totalProductiveDuration += usage.TotalSeconds;
                                        break;
                                    case "Unproductive":
                                        totalUnproductiveDuration += usage.TotalSeconds;
                                        break;
                                    case "Neutral":
                                        totalNeutralDuration += usage.TotalSeconds;
                                        break;
                                }
                            }
                        }
                    }
                    
                }
                GrandtotalProductiveDuration += totalProductiveDuration;
   //             SUM(
   //    (CAST(SUBSTRING(A.Total_Time, 1, CHARINDEX(':', A.Total_Time) - 1) AS INT) * 3600) +
   //    (CAST(SUBSTRING(A.Total_Time, CHARINDEX(':', A.Total_Time) + 1, LEN(A.Total_Time)) AS INT) * 60)
   //) AS Total_Seconds
                var query = @"SELECT 
    SUM(DATEDIFF(SECOND, A.[Start_Time], A.[End_Time])) AS Total_Seconds
FROM Attendance A
INNER JOIN Users U ON U.Id = A.UserId
INNER JOIN Team T ON T.Id = U.TeamId
INNER JOIN Organization O ON O.Id = T.OrganizationId
WHERE O.Id = @organizationId
  AND (@teamId IS NULL OR T.Id = @teamId)
  AND A.AttendanceDate >= @fromDate
  AND A.AttendanceDate <= @toDate;";

                var totalseconds = await _dapper.QueryFirstOrDefaultAsync<long?>(
                    query,
                    new { organizationId, teamId, fromDate, toDate }
                );

                var totalDurationInSeconds = totalProductiveDuration;

                //GrandtotalTimeDuration = GrandtotalTimeDuration + totalProductiveDuration;

                if (totalDurationInSeconds > 0)
                {

                    var productivePercent = (double)totalProductiveDuration / totalseconds * 100;
                    if (productivePercent > 100)
                    {
                        productivePercent = 100;

                    }


                    var teamResult = new
                    {
                        productive_duration = totalProductiveDuration,
                        unproductive_duration = totalUnproductiveDuration,
                        neutral_duration = totalNeutralDuration,
                        total_duration = totalDurationInSeconds,
                        productive_percent = productivePercent,
                        team_name = team.TeamName
                    };

                    teamResults.Add(teamResult);
                }
            }
            
            // Sort teams by productive percentage
            var sortedTeams = teamResults.OrderByDescending(t => t.productive_duration).ToList();

            // Identify the top team(s)
            var topTeams = sortedTeams.TakeWhile(t => t.productive_percent == sortedTeams.First().productive_percent).ToList();

            var leastProductivePercent = sortedTeams.LastOrDefault()?.productive_duration ?? 0;

            var bottomTeams = sortedTeams.Where(t => t.productive_duration == leastProductivePercent).ToList();

            TimeSpan time = TimeSpan.FromSeconds(GrandtotalProductiveDuration);
            string formattedTime = time.ToString(@"hh\:mm\:ss");
            var GrandTotalpercentage = GrandtotalTimeDuration > 0 ?
                                     (double)GrandtotalProductiveDuration / GrandtotalTimeDuration * 100 : 0.0;

            return new
            {
                GrandTotalpercentage = GrandTotalpercentage,
                GrandtotalTimeDuration = formattedTime,
                data = new
                {
                    top = topTeams,
                    bottom = bottomTeams
                }
            };

        }

        public async Task<dynamic> GetTotal_Working_Time(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            string appUsageQuery = @"
            SELECT 
                  A.AttendanceDate as start_timing, 
                   SUM(DATEDIFF(SECOND, '00:00:00', A.Total_Time)) AS TotalTimes,
                   ISNULL(SUM(DATEDIFF(SECOND, A.Start_Time, A.End_Time)), 0) AS PunchDuration
             FROM  
                  Attendance A
             INNER JOIN 
                   Users Us ON A.UserId = Us.Id
             INNER JOIN 
                   Team U ON Us.TeamId = U.Id
             INNER JOIN 
                   Organization O ON U.OrganizationId = O.Id
             WHERE 
                   O.Id = @OrganizationId 
                   AND (@UserId IS NULL OR Us.Id = @UserId)
                   AND A.AttendanceDate BETWEEN @FromDate AND @ToDate
                   AND (@TeamId IS NULL OR U.Id = @TeamId)
             GROUP BY  
                   A.AttendanceDate;";
            string appUsageQuery1 = @"
                  SELECT 
                      BR.BreakDate, 
                      ISNULL(SUM(DATEDIFF(SECOND, BR.Start_Time, BR.End_Time)), 0) AS break_duration
                  FROM  
                       BreakEntry BR
                  INNER JOIN 
                       Users Us ON BR.UserId = Us.Id
                  INNER JOIN 
                       Team U ON Us.TeamId = U.Id
                  INNER JOIN 
                       Organization O ON U.OrganizationId = O.Id
                  WHERE 
                       O.Id = @OrganizationId 
                       AND (@UserId IS NULL OR Us.Id = @UserId)
                       AND CAST(BR.BreakDate AS DATE) BETWEEN @FromDate AND @ToDate
                       AND (@TeamId IS NULL OR U.Id = @TeamId)
                 GROUP BY  
                      BR.BreakDate;";

            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                FromDate = fromDate,
                ToDate = toDate
            };

            var appUsages = await _dapper.GetAllAsync<dynamic>(appUsageQuery, parameters);
            var appUsages1 = await _dapper.GetAllAsync<dynamic>(appUsageQuery1, parameters);


            var dateRange = Enumerable.Range(0, (toDate - fromDate).Days + 1)
                                      .Select(d => fromDate.AddDays(d))
                                      .ToList();

            var mergedData = dateRange.Select(date =>
            {
                var attendance = appUsages.FirstOrDefault(a => a.start_timing.Date == date.Date);
                var breakData = appUsages1.FirstOrDefault(b => b.BreakDate.Date == date.Date);

                var punchDuration = attendance?.PunchDuration ?? 0;
                var totalTimes = attendance?.TotalTimes ?? 0;
                var breakDuration = breakData?.break_duration ?? 0;

                // Convert the durations from seconds to TimeSpan
                var punchDurationTimeSpan = TimeSpan.FromSeconds(punchDuration);
                var breakDurationTimeSpan = TimeSpan.FromSeconds(breakDuration);
                var activeDurationTimeSpan = TimeSpan.FromSeconds(punchDuration - breakDuration);

                if (punchDuration > 0)
                {
                    return new
                    {
                        start_timing = date.ToString("yyyy-MM-dd"),
                        punch_duration = punchDurationTimeSpan.ToString(@"hh\:mm\:ss"),
                        break_duration = breakDurationTimeSpan.ToString(@"hh\:mm\:ss"),
                        active_duration = activeDurationTimeSpan.ToString(@"hh\:mm\:ss"),
                    };
                }
                return null;
            }).Where(d => d != null).ToList();

            return new { data = mergedData };

        }
        public async Task<List<AppUsage>> GetAppUsagesS(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
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
                   Team T ON T.Id = U.TeamId
           INNER JOIN 
               Organization O ON U.OrganizationId = O.Id
           WHERE  
                  O.Id = @OrganizationId 
                  AND A.UsageDate BETWEEN @FromDate AND @ToDate
                  AND (@TeamId IS NULL OR T.Id = @TeamId)
                  AND (@UserId IS NULL OR A.UserId = @UserId)
           GROUP BY 
               A.UserId, 
               A.ApplicationName, 
               A.Details, 
               A.UsageDate;
        ";

            var urlUsageQuery = @"
    SELECT 
        U.UserId,
        U.Url AS ApplicationName,
        NULL AS Details,
        SUM(DATEDIFF(SECOND, '00:00:00', U.TotalUsage)) AS TotalSeconds,
        U.UsageDate
    FROM 
        UrlUsage U
    INNER JOIN 
        Users Us ON U.UserId = Us.Id
    INNER JOIN 
        Team T ON T.Id = Us.TeamId
    INNER JOIN 
        Organization O ON Us.OrganizationId = O.Id
    WHERE 
        O.Id = @OrganizationId 
        AND U.UsageDate BETWEEN @FromDate AND @ToDate
        AND (@TeamId IS NULL OR T.Id = @TeamId)
        AND (@UserId IS NULL OR U.UserId = @UserId)
    GROUP BY 
        U.UserId,
        U.Url,
        U.UsageDate;
";
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

            var allUsages = appUsages.Concat(urlUsages);

            // Group by ApplicationName (URL or Application) to handle duplicates
            var groupedUsages = allUsages
         .GroupBy(u => new { u.ApplicationName, u.UsageDate }) // Grouping by both ApplicationName and UsageDate
         .Select(g => new AppUsage
         {
             UserId = g.First().UserId, // Choose how to handle UserId if there are multiple (e.g., take the first one)
             ApplicationName = g.Key.ApplicationName, // Use the grouped ApplicationName
             Details = g.First().Details, // Keep the first Details (adjust as needed)
             TotalSeconds = g.Sum(u => u.TotalSeconds), // Sum the TotalSeconds for the same ApplicationName and UsageDate
             UsageDate = g.Key.UsageDate // Use the UsageDate from the group key
         })
         .ToList();

            // Return the merged and grouped list
            return groupedUsages;


        }
        public async Task<dynamic> GetProductivity_Trend(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var teamQuery = @"
       SELECT T.Id
       FROM Team T
       INNER JOIN Organization O ON T.OrganizationId = O.Id
       WHERE O.Id = @OrganizationId
       AND (@TeamId IS NULL OR T.Id = @TeamId) ";

            var teams = await _dapper.GetAllAsync<int>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });

            var dateWiseDurations = new List<DailyProductivityDuration>();

            foreach (var team in teams)
            {
                teamId = team;
                var usages = await GetAppUsagesS(organizationId, teamId, userId, fromDate, toDate);

                // Initialize a dictionary to hold data by date
                var dailyDurations = new Dictionary<DateTime, DailyProductivityDuration>();

                foreach (var usage in usages)
                {
                    usage.ApplicationName = usage.ApplicationName.ToLower();

                    if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge" && usage.ApplicationName != "firefox" && usage.ApplicationName != "opera")
                    {
                        // Query for category and productivity details
                        var imbuildAppQuery = @"
                   SELECT CategoryId 
                   FROM ImbuildAppsAndUrls 
                   WHERE Name LIKE '%' + @ApplicationName + '%'";
                        var categoryId = await _dapper.QueryFirstOrDefaultAsync<int?>(imbuildAppQuery, new { ApplicationName = usage.ApplicationName });

                        if (categoryId.HasValue)
                        {
                            usage.CategoryId = categoryId.Value;

                            var categoryQuery = @"
                   SELECT CategoryName, ProductivityId 
                   FROM Categories 
                   WHERE Id = @CategoryId";

                            var category = await _dapper.QueryFirstOrDefaultAsync<(string CategoryName, int ProductivityId)>(categoryQuery, new { CategoryId = categoryId.Value });

                            if (category != default)
                            {
                                usage.CategoryName = category.CategoryName;

                                // Fetch ProductivityName from ProductivityAssign
                                var productivityQuery = @"
                       SELECT Name FROM ProductivityAssign
                       WHERE Id = @ProductivityId";

                                var productivityName = await _dapper.QueryFirstOrDefaultAsync<string>(productivityQuery, new { ProductivityId = category.ProductivityId });
                                usage.ProductivityName = productivityName;

                                // Get the date from the usage
                                var usageDate = usage.UsageDate.Date;  // Just the date part, not the time

                                // Ensure the date exists in the dictionary
                                if (!dailyDurations.ContainsKey(usageDate))
                                {
                                    dailyDurations[usageDate] = new DailyProductivityDuration
                                    {
                                        Date = usageDate.ToString("yyyy-MM-dd")
                                    };
                                }

                                // Add to the corresponding day's duration
                                var dailyData = dailyDurations[usageDate];

                                switch (usage.ProductivityName)
                                {
                                    case "Productive":
                                        dailyData.ProductiveDuration += usage.TotalSeconds;
                                        break;
                                    case "Unproductive":
                                        dailyData.UnproductiveDuration += usage.TotalSeconds;
                                        break;
                                    case "Neutral":
                                        dailyData.NeutralDuration += usage.TotalSeconds;
                                        break;
                                }

                                // Also add to the total duration for the day
                                dailyData.TotalDuration += usage.TotalSeconds;
                            }
                        }
                    }
                }

                // Add all days' data to the result list
                dateWiseDurations.AddRange(dailyDurations.Values);
            }

            // Group by Date and Sum the TotalDurations for each date
            var aggregatedDurations = dateWiseDurations
                .GroupBy(d => d.Date) // Group by date
                .Select(g => new DailyProductivityDuration
                {
                    Date = g.Key, // Use the date from the group
                    TotalDuration = g.Sum(d => d.TotalDuration), // Sum all total durations
                    ProductiveDuration = g.Sum(d => d.ProductiveDuration), // Sum all productive durations
                    UnproductiveDuration = g.Sum(d => d.UnproductiveDuration), // Sum all unproductive durations
                    NeutralDuration = g.Sum(d => d.NeutralDuration) // Sum all neutral durations
                })
                .ToList();

            var filteredDurations = aggregatedDurations
                .Where(d => d.TotalDuration > 0)
                .OrderBy(d => d.Date)
                .ToList();


            foreach (var duration in filteredDurations)
            {
                // Format durations to hh:mm:ss and assign to the corresponding properties
                duration.Total_Duration = TimeSpan.FromSeconds(duration.TotalDuration).ToString(@"hh\:mm\:ss");
                duration.Productive_Duration = TimeSpan.FromSeconds(duration.ProductiveDuration).ToString(@"hh\:mm\:ss");
                duration.Unproductive_Duration = TimeSpan.FromSeconds(duration.UnproductiveDuration).ToString(@"hh\:mm\:ss");
                duration.Neutral_Duration = TimeSpan.FromSeconds(duration.NeutralDuration).ToString(@"hh\:mm\:ss");
            }

            // Return the date-wise durations with the formatted durations
            return filteredDurations.Select(duration => new
            {
                date = duration.Date,
                total_Duration = duration.Total_Duration,
                productive_Duration = duration.Productive_Duration,
                unproductive_Duration = duration.Unproductive_Duration,
                neutral_Duration = duration.Neutral_Duration
            }).ToList();
        }


        public async Task<List<dynamic>> GetAppUsagesSS(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            var appUsageQuery = @"
SELECT 
    A.[UserId], 
    CONCAT(u.[First_Name], ' ', u.[Last_Name]) AS FullName, 
    COUNT(DISTINCT CONVERT(DATE, A.[AttendanceDate])) AS AttendanceCount,
    MIN(A.[AttendanceDate]) AS StartDate, 
    MAX(A.[AttendanceDate]) AS EndDate,
    SUM(DATEDIFF(SECOND, A.[Start_Time], A.[End_Time])) AS ActiveTimeInSeconds
FROM 
    [EMP4].[dbo].[Attendance] A
INNER JOIN 
    Users U ON U.id = A.UserId
INNER JOIN 
        Team T ON T.Id = U.TeamId
INNER JOIN 
        Organization O ON U.OrganizationId = O.Id
WHERE 
    O.Id = @OrganizationId
    AND (@TeamId IS NULL OR T.Id = @TeamId)
    AND (@UserId IS NULL OR A.UserId = @UserId)
    AND A.[AttendanceDate] >= @fromDate 
    AND A.[AttendanceDate] < DATEADD(DAY, 1, @toDate)
    AND A.[Start_Time] IS NOT NULL 
    AND A.[End_Time] IS NOT NULL
GROUP BY 
    A.[UserId], u.[First_Name], u.[Last_Name]";

            var breakdetails = @"
    SELECT 
    br.UserId,
    SUM(
        CASE 
            WHEN br.Start_Time IS NOT NULL AND br.End_Time IS NOT NULL 
            THEN DATEDIFF(SECOND, br.Start_Time, br.End_Time) 
            ELSE 0 
        END
    ) AS TotalBreakDurationInSeconds
   
FROM 
    BreakEntry br
INNER JOIN 
    Users U ON U.id = br.UserId
INNER JOIN 
        Team T ON T.Id = U.TeamId
INNER JOIN 
        Organization O ON U.OrganizationId = O.Id
WHERE 
    CONVERT(DATE, br.BreakDate) BETWEEN @FromDate AND @ToDate
    And O.Id = @OrganizationId
    AND (@TeamId IS NULL OR T.Id = @TeamId)
    AND (@UserId IS NULL OR br.UserId = @UserId)
GROUP BY 
    br.UserId
ORDER BY 
   br.UserId;";
            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                FromDate = fromDate,
                ToDate = toDate
            };


            var appUsages = await _dapper.GetAllAsync<dynamic>(appUsageQuery, parameters);
            var breakDetails = await _dapper.GetAllAsync<dynamic>(breakdetails, parameters);

            var result = (from app in appUsages
                          join breakDetail in breakDetails on app.UserId equals breakDetail.UserId into breakGroup
                          from bg in breakGroup.DefaultIfEmpty()
                          select new
                          {
                              UserId = app.UserId,
                              FullName = app.FullName,
                              AttendanceCount = app.AttendanceCount,
                              ActiveTimeInSeconds = app.ActiveTimeInSeconds,
                              TotalBreakDurationInSeconds = bg != null ? bg.TotalBreakDurationInSeconds : 0,
                              OnlineDurationInHours = app.ActiveTimeInSeconds - (bg != null ? bg.TotalBreakDurationInSeconds : 0)
                          })
               .Cast<dynamic>() 
               .ToList();

            var getUsers = @"
                SELECT id AS UserId, CONCAT(First_Name, ' ', Last_Name) AS FullName 
                FROM Users 
                WHERE TeamId = @TeamId
                AND (@UserId IS NULL OR Id = @UserId)";

            var getUsers1 = await _dapper.GetAllAsync<dynamic>(getUsers, parameters);

            // Assuming 'usagess' and 'getUsers1' are lists of the same or compatible type
            // Merge and process results
            var tt = result.Concat(getUsers1)
                .GroupBy(item => item.UserId)
                .Select(group =>
                {
                    var usageEntry = result.FirstOrDefault(u => u.UserId == group.Key);
                    return usageEntry ?? group.First();
                })
                .ToList();

            return tt;


        }
        public async Task<dynamic> GetEmployeeList(int organizationId, int? teamId, [FromQuery] int? userId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
           

            var teamQuery = @"
                         SELECT T.Id
                          FROM Team T
                          INNER JOIN Organization O ON T.OrganizationId = O.Id
                          WHERE O.Id = @OrganizationId
                          AND (@TeamId IS NULL OR T.Id = @TeamId) ";

            var teams = await _dapper.GetAllAsync<int>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });

            var result = new List<dynamic>();

            if (userId.HasValue)
            {
                foreach (var team in teams)
                {

                    teamId = team;
                    var usagess = await GetAppUsagesSS(organizationId, teamId, userId, fromDate, toDate);

                    foreach (var us in usagess)
                    {
                        var userIdd = us.UserId;
                        var FullANme = us.FullName;

                        int totalProductiveDuration = 0;
                        int totalUnproductiveDuration = 0;
                        int totalNeutralDuration = 0;

                        var usages = await GetAppUsages(organizationId, teamId, userId, fromDate, toDate);

                        var totalUsages = usages
                        .GroupBy(u => u.ApplicationName)
                        .Select(g => new { ApplicationName = g.Key, TotalSeconds = g.Sum(u => u.TotalSeconds) })
                        .ToDictionary(t => t.ApplicationName, t => t.TotalSeconds);

                        foreach (var usage in usages)
                        {
                            usage.ApplicationName = usage.ApplicationName.ToLower();

                            if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge" && usage.ApplicationName != "firefox" && usage.ApplicationName != "opera")
                            {
                                if (totalUsages.TryGetValue(usage.ApplicationName, out var totalSeconds))
                                {
                                    usage.TotalSeconds = totalSeconds;
                                    usage.TotalUsage = TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");
                                }

                                var imbuildAppQuery = @"
                        SELECT CategoryId 
                         FROM ImbuildAppsAndUrls 
                        WHERE Name LIKE '%' + @ApplicationName + '%'";
                                var categoryId = await _dapper.QueryFirstOrDefaultAsync<int?>(imbuildAppQuery, new { ApplicationName = usage.ApplicationName });

                                if (categoryId.HasValue)
                                {
                                    usage.CategoryId = categoryId.Value;

                                    var categoryQuery = @"
                        SELECT CategoryName, ProductivityId 
                        FROM Categories 
                         WHERE Id = @CategoryId";

                                    var category = await _dapper.QueryFirstOrDefaultAsync<(string CategoryName, int ProductivityId)>(categoryQuery, new { CategoryId = categoryId.Value });

                                    if (category != default)
                                    {
                                        usage.CategoryName = category.CategoryName;

                                        var productivityQuery = @"
                            SELECT Name FROM ProductivityAssign
                            WHERE Id = @ProductivityId";

                                        var productivityName = await _dapper.QueryFirstOrDefaultAsync<string>(productivityQuery, new { ProductivityId = category.ProductivityId });
                                        usage.ProductivityName = productivityName;

                                        switch (usage.ProductivityName)
                                        {
                                            case "Productive":
                                                totalProductiveDuration += usage.TotalSeconds;
                                                break;
                                            case "Unproductive":
                                                totalUnproductiveDuration += usage.TotalSeconds;
                                                break;
                                            case "Neutral":
                                                totalNeutralDuration += usage.TotalSeconds;
                                                break;
                                        }
                                    }
                                }
                                //else
                                //{
                                //    // Assign to Neutral if no category is found
                                //    totalNeutralDuration += usage.TotalSeconds;
                                //}
                            }
                        }
                        var AttendanceCount = us.AttendanceCount;
                        var onlineDurationInSeconds = us.OnlineDurationInHours;
                        var activeDurationInSeconds = us.ActiveTimeInSeconds;
                        var breakDurationInSeconds = us.TotalBreakDurationInSeconds;
                        var totalDurationInSeconds = totalProductiveDuration + totalUnproductiveDuration + totalNeutralDuration;

                        var dateDifferenceInDays = (toDate - fromDate).TotalDays;

                        if (dateDifferenceInDays <= 0)
                        {
                            dateDifferenceInDays = 1;
                        }
                        var percentageProductiveDuration = ((double)totalProductiveDuration / activeDurationInSeconds) * 100;


                        var dynamicItem = new ExpandoObject() as dynamic;

                        dynamicItem.UserID = userIdd;
                        dynamicItem.full_Name = FullANme;
                        dynamicItem.AttendanceCount = AttendanceCount;

                        //dynamicItem.active_duration = TimeSpan.FromSeconds(activeDurationInSeconds).ToString(@"hh\:mm\:ss"); 
                        //dynamicItem.break_duration = TimeSpan.FromSeconds(breakDurationInSeconds).ToString(@"hh\:mm\:ss");
                        //dynamicItem.online_duration = TimeSpan.FromSeconds(onlineDurationInSeconds).ToString(@"hh\:mm\:ss");

                        //dynamicItem.TotalProductiveDuration = TimeSpan.FromSeconds(totalProductiveDuration).ToString(@"hh\:mm\:ss");
                        //dynamicItem.TotalUnproductiveDuration = TimeSpan.FromSeconds(totalUnproductiveDuration).ToString(@"hh\:mm\:ss");
                        //dynamicItem.TotalNeutralDuration = TimeSpan.FromSeconds(totalNeutralDuration).ToString(@"hh\:mm\:ss");
                        //dynamicItem.TotalDuration = TimeSpan.FromSeconds(totalDurationInSeconds).ToString(@"hh\:mm\:ss");
                        //dynamicItem.PercentageProductiveDuration = percentageProductiveDuration;

                        //result.Add(dynamicItem);
                        dynamicItem.ActiveDuration = TimeSpan.FromSeconds(Convert.ToDouble(activeDurationInSeconds)).ToString(@"hh\:mm\:ss");
                        dynamicItem.BreakDuration = TimeSpan.FromSeconds(Convert.ToDouble(breakDurationInSeconds)).ToString(@"hh\:mm\:ss");
                        dynamicItem.OnlineDuration = TimeSpan.FromSeconds(Convert.ToDouble(onlineDurationInSeconds)).ToString(@"hh\:mm\:ss");

                        dynamicItem.TotalProductiveDuration = TimeSpan.FromSeconds(totalProductiveDuration).ToString(@"hh\:mm\:ss");
                        dynamicItem.TotalUnproductiveDuration = TimeSpan.FromSeconds(totalUnproductiveDuration).ToString(@"hh\:mm\:ss");
                        dynamicItem.TotalNeutralDuration = TimeSpan.FromSeconds(totalNeutralDuration).ToString(@"hh\:mm\:ss");
                        dynamicItem.TotalDuration = TimeSpan.FromSeconds(totalDurationInSeconds).ToString(@"hh\:mm\:ss");
                        dynamicItem.PercentageProductiveDuration = percentageProductiveDuration;

                        result.Add(dynamicItem);
                    }

                    
                }

                return result;
            }
            else
            {
                foreach (var team in teams)
                {

                    teamId = team;
                    var usagess = await GetAppUsagesSS(organizationId, teamId, userId, fromDate, toDate);


                    foreach (var us in usagess)
                    {
                        var userIdd = us.UserId;
                        var FullANme = us.FullName;

                        int totalProductiveDuration = 0;
                        int totalUnproductiveDuration = 0;
                        int totalNeutralDuration = 0;

                        var usages = await GetAppUsages(organizationId, teamId, userId, fromDate, toDate);

                        var totalUsages = usages
                        .GroupBy(u => u.ApplicationName)
                        .Select(g => new { ApplicationName = g.Key, TotalSeconds = g.Sum(u => u.TotalSeconds) })
                        .ToDictionary(t => t.ApplicationName, t => t.TotalSeconds);

                        foreach (var usage in usages)
                        {
                            usage.ApplicationName = usage.ApplicationName.ToLower();

                            if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge" && usage.ApplicationName != "firefox" && usage.ApplicationName != "opera")
                            {
                                if (totalUsages.TryGetValue(usage.ApplicationName, out var totalSeconds))
                                {
                                    usage.TotalSeconds = totalSeconds;
                                    usage.TotalUsage = TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");
                                }

                                var imbuildAppQuery = @"
                        SELECT CategoryId 
                         FROM ImbuildAppsAndUrls 
                        WHERE Name LIKE '%' + @ApplicationName + '%'";
                                var categoryId = await _dapper.QueryFirstOrDefaultAsync<int?>(imbuildAppQuery, new { ApplicationName = usage.ApplicationName });

                                if (categoryId.HasValue)
                                {
                                    usage.CategoryId = categoryId.Value;

                                    var categoryQuery = @"
                        SELECT CategoryName, ProductivityId 
                        FROM Categories 
                         WHERE Id = @CategoryId";

                                    var category = await _dapper.QueryFirstOrDefaultAsync<(string CategoryName, int ProductivityId)>(categoryQuery, new { CategoryId = categoryId.Value });

                                    if (category != default)
                                    {
                                        usage.CategoryName = category.CategoryName;

                                        var productivityQuery = @"
                            SELECT Name FROM ProductivityAssign
                            WHERE Id = @ProductivityId";

                                        var productivityName = await _dapper.QueryFirstOrDefaultAsync<string>(productivityQuery, new { ProductivityId = category.ProductivityId });
                                        usage.ProductivityName = productivityName;

                                        switch (usage.ProductivityName)
                                        {
                                            case "Productive":
                                                totalProductiveDuration += usage.TotalSeconds;
                                                break;
                                            case "Unproductive":
                                                totalUnproductiveDuration += usage.TotalSeconds;
                                                break;
                                            case "Neutral":
                                                totalNeutralDuration += usage.TotalSeconds;
                                                break;
                                        }
                                    }
                                }
                                //else
                                //{
                                //    // Assign to Neutral if no category is found
                                //    totalNeutralDuration += usage.TotalSeconds;
                                //}
                            }
                        }
                        var AttendanceCount = us.AttendanceCount;
                        var onlineDurationInSeconds = us.OnlineDurationInHours;
                        var activeDurationInSeconds = us.ActiveTimeInSeconds;
                        var breakDurationInSeconds = us.TotalBreakDurationInSeconds;
                        var totalDurationInSeconds = totalProductiveDuration + totalUnproductiveDuration + totalNeutralDuration;

                        var dateDifferenceInDays = (toDate - fromDate).TotalDays;

                        if (dateDifferenceInDays <= 0)
                        {
                            dateDifferenceInDays = 1;
                        }
                        var percentageProductiveDuration = ((double)totalProductiveDuration / activeDurationInSeconds) * 100;


                        var dynamicItem = new ExpandoObject() as dynamic;

                        dynamicItem.UserID = userIdd;
                        dynamicItem.full_Name = FullANme;
                        dynamicItem.AttendanceCount = AttendanceCount;

                        //dynamicItem.active_duration = TimeSpan.FromSeconds(activeDurationInSeconds).ToString(@"hh\:mm\:ss"); 
                        //dynamicItem.break_duration = TimeSpan.FromSeconds(breakDurationInSeconds).ToString(@"hh\:mm\:ss");
                        //dynamicItem.online_duration = TimeSpan.FromSeconds(onlineDurationInSeconds).ToString(@"hh\:mm\:ss");

                        //dynamicItem.TotalProductiveDuration = TimeSpan.FromSeconds(totalProductiveDuration).ToString(@"hh\:mm\:ss");
                        //dynamicItem.TotalUnproductiveDuration = TimeSpan.FromSeconds(totalUnproductiveDuration).ToString(@"hh\:mm\:ss");
                        //dynamicItem.TotalNeutralDuration = TimeSpan.FromSeconds(totalNeutralDuration).ToString(@"hh\:mm\:ss");
                        //dynamicItem.TotalDuration = TimeSpan.FromSeconds(totalDurationInSeconds).ToString(@"hh\:mm\:ss");
                        //dynamicItem.PercentageProductiveDuration = percentageProductiveDuration;

                        //result.Add(dynamicItem);
                        dynamicItem.ActiveDuration = TimeSpan.FromSeconds(Convert.ToDouble(activeDurationInSeconds)).ToString(@"hh\:mm\:ss");
                        dynamicItem.BreakDuration = TimeSpan.FromSeconds(Convert.ToDouble(breakDurationInSeconds)).ToString(@"hh\:mm\:ss");
                        dynamicItem.OnlineDuration = TimeSpan.FromSeconds(Convert.ToDouble(onlineDurationInSeconds)).ToString(@"hh\:mm\:ss");

                        dynamicItem.TotalProductiveDuration = TimeSpan.FromSeconds(totalProductiveDuration).ToString(@"hh\:mm\:ss");
                        dynamicItem.TotalUnproductiveDuration = TimeSpan.FromSeconds(totalUnproductiveDuration).ToString(@"hh\:mm\:ss");
                        dynamicItem.TotalNeutralDuration = TimeSpan.FromSeconds(totalNeutralDuration).ToString(@"hh\:mm\:ss");
                        dynamicItem.TotalDuration = TimeSpan.FromSeconds(totalDurationInSeconds).ToString(@"hh\:mm\:ss");
                        dynamicItem.PercentageProductiveDuration = percentageProductiveDuration;

                        result.Add(dynamicItem);
                    }

                    userId = null;
                }

                return result;

            }

        }
    }
}

