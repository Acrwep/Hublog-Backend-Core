using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Repository.Entities.Model.UrlModel;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
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
        public async Task<List<MappingModel>> GetImbuildAppsAndUrls(int OrganizationId,string userSearchQuery, string type, string category)
        {

            var query = "Sp_GetImbuildAppsAndUrls";

           var parameters = new
            {
                OrganizationId= OrganizationId,
                userSearchQuery = userSearchQuery,
                type = type,
                category = category
            };

            var result=await _dapper.GetAllAsync<MappingModel>(query, parameters);
            return result;
        }
        public async Task<List<MappingModel>> GetByIdImbuildAppsAndUrls(int id)
        {
            var query = @"
        SELECT [id], [type], [name], [categoryid]
        FROM ImbuildAppsAndUrls
        WHERE [id] = @Id";

            var parameters = new { Id = id };
            var result = await _dapper.GetAllAsync<MappingModel>(query, parameters);

            return result;
        }

        public async Task<bool> InsertImbuildAppsAndUrls(int id, MappingModel model)
        {
            var query = @"
                UPDATE ImbuildAppsAndUrls
                SET [CategoryId] = @NewCategoryId
                WHERE [id] = @Id";

            var parameters = new { Id = id, NewCategoryId = model.CategoryId };
            var affectedRows = await _dapper.ExecuteAsync(query, parameters);

            return affectedRows > 0;
        }
        public async Task<bool> AddImbuildAppsAndUrls(MappingModel mappingModel)
        {
            var query = @"
        INSERT INTO [ImbuildAppsAndUrls] ([Name], [Type], [CategoryId],[OrganizationId])
        VALUES (@Name, @Type, @CategoryId,@OrganizationId)";

            var parameters = new
            {
                Name = mappingModel.Name,
                Type = mappingModel.Type,
                CategoryId = mappingModel.CategoryId,
                OrganizationId= mappingModel.OrganizationId
            };

            try
            {
                var affectedRows = await _dapper.ExecuteAsync(query, parameters);
                return affectedRows > 0;
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) // Unique constraint violation numbers
            {
                throw new Exception($"The name '{mappingModel.Name}' already exists. Please use a different name.");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the record.", ex);
            }
        }


        public async Task<List<AppUsage>> GetAppUsages(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                FromDate = fromDate,
                ToDate = toDate
            };

            return (await _dapper.GetAllAsyncs<AppUsage>("Sp_GetAppUsageEmployeeList", parameters, commandType: CommandType.StoredProcedure)).ToList();
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
                // var usages = await GetAppUsages(organizationId, teamId, userId, fromDate, toDate);
                var parameterss = new
                {
                    OrganizationId = organizationId,
                    TeamId = teamId,
                    UserId = userId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                var usages = await _dapper.GetAllAsyncs<AppUsage>("Sp_GetAppUsageEmployeeList", parameterss, commandType: CommandType.StoredProcedure);


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

                    if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge")
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

                string FormatDuration(long totalSeconds)
                {
                    var hours = totalSeconds / 3600; // Total hours
                    var minutes = (totalSeconds % 3600) / 60; // Remaining minutes
                    var seconds = totalSeconds % 60; // Remaining seconds
                    return $"{hours:D2}:{minutes:D2}:{seconds:D2}"; // Format as "HH:mm:ss"
                }

                result = new ProductivityDurations
                {
                    TotalProductiveDuration = FormatDuration(totalProductiveDuration),
                    TotalUnproductiveDuration = FormatDuration(totalUnproductiveDuration),
                    TotalNeutralDuration = FormatDuration(totalNeutralDuration),
                    TotalDuration = FormatDuration(totalDurationInSeconds),
                    AverageDuratiopn = FormatDuration((long)averageDurationInSeconds)
                };

            }
            return result;

        }

        public async Task<List<AppUsage>> GetAppUsages(int organizationId, int? teamId, DateTime fromDate, DateTime toDate)
        {
            string appUsageQuery = "GetAppUsage";
            string urlUsageQuery = "GetUrlUsage";

            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
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
                //var usages = await GetAppUsages(organizationId, teamId, fromDate, toDate);
                var urlUsageQuery = "Get_App_Url_Data";
                var parameters = new
                {
                    OrganizationId = organizationId,
                    TeamId = teamId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                IEnumerable<App_UrlModel> usages = await _dapper.GetAllAsync<App_UrlModel>(urlUsageQuery, parameters);

                int totalProductiveDuration = 0, totalUnproductiveDuration = 0, totalNeutralDuration = 0;

                foreach (var usage in usages)
                {
                    usage.Name = usage.Name.ToLower();

                    if (usage.Name != "chrome" && usage.Name != "msedge")
                    {
                        var parameters1 = new { ApplicationName = usage.Name.ToLower() };
                        var app = await _dapper.QueryFirstOrDefaultAsync<string>("GetApplicationCategoryAndProductivity",
                            parameters1,
                            commandType: CommandType.StoredProcedure
                        );
                        if (app != null)
                        {
                            switch (app)
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

                var totalDurationInSeconds = totalProductiveDuration + totalUnproductiveDuration + totalNeutralDuration;

                result.Add(new TeamProductivity
                {
                    TeamName = team.TeamName,
                    TotalProductiveDuration = FormatDuration(totalProductiveDuration),
                    TotalNeutralDuration = FormatDuration(totalNeutralDuration),
                    TotalUnproductiveDuration = FormatDuration(totalUnproductiveDuration),
                    TotalDuration = FormatDuration(totalDurationInSeconds),
                });
            }

            return result;
        }
        private string FormatDuration(long totalSeconds)
        {
            var hours = totalSeconds / 3600;
            var minutes = (totalSeconds % 3600) / 60;
            var seconds = totalSeconds % 60;
            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
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
            long abc =0;
            foreach (var team in teams)
            {
                teamId = team.TeamId;
                //var usages = await GetAppUsages(organizationId, teamId, fromDate, toDate);
                var urlUsageQuery = "Get_App_Url_Data";
                var parameters = new
                {
                    OrganizationId = organizationId,
                    TeamId = teamId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                IEnumerable<App_UrlModel> usages = await _dapper.GetAllAsync<App_UrlModel>(urlUsageQuery, parameters);

                int totalProductiveDuration = 0, totalUnproductiveDuration = 0, totalNeutralDuration = 0;

                foreach (var usage in usages)
                {
                    usage.Name = usage.Name.ToLower();

                    if (usage.Name != "chrome" && usage.Name != "msedge")
                    {
                        var parameters1 = new { ApplicationName = usage.Name.ToLower() };
                        var app = await _dapper.QueryFirstOrDefaultAsync<string>("GetApplicationCategoryAndProductivity",
                            parameters1,
                            commandType: CommandType.StoredProcedure
                        );
                        if (app != null)
                        {
                            switch (app)
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
                GrandtotalProductiveDuration += totalProductiveDuration;
                //var Totalproductivitypercentage = totalProductiveDuration + totalUnproductiveDuration + totalNeutralDuration;
                var query = @"SELECT 
    COALESCE(SUM( DATEDIFF(SECOND, 0, TRY_CONVERT(TIME, a.Total_Time)) ), 0) AS Total_Seconds
FROM Attendance A
INNER JOIN Users U ON U.Id = A.UserId
INNER JOIN Team T ON T.Id = U.TeamId
INNER JOIN Organization O ON O.Id = T.OrganizationId
WHERE O.Id = @organizationId
  AND (@teamId IS NULL OR T.Id = @teamId)
  AND a.Total_Time IS NOT NULL   
  AND A.AttendanceDate >= @fromDate
  AND A.AttendanceDate <= @toDate;";

                var totalseconds = await _dapper.QueryFirstOrDefaultAsync<long?>(
                    query,
                    new { organizationId, teamId, fromDate, toDate }
                );

                var totalDurationInSeconds = totalProductiveDuration;

                abc = abc + (totalseconds ?? 0);

                if (totalDurationInSeconds > 0)
                {
                    //double productivePercent = ((double)totalProductiveDuration/Totalproductivitypercentage) * 100;
                    double productivePercent = (double)totalProductiveDuration / totalseconds.GetValueOrDefault() * 100;
                    if (productivePercent > 100)
                    {
                        productivePercent = 100;

                    }

                    var teamResult = new
                    {
                        productive_duration = FormatDuration(totalProductiveDuration),
                        unproductive_duration = FormatDuration(totalUnproductiveDuration),
                        neutral_duration = FormatDuration(totalNeutralDuration),
                        total_duration = FormatDuration(totalDurationInSeconds),
                        productive_percent = productivePercent,
                        team_name = team.TeamName
                    };

                    teamResults.Add(teamResult);
                }
            }


            var sortedTeams = teamResults.OrderByDescending(t => t.productive_percent).ToList();
            var topTeams = sortedTeams.Take(3).ToList();

            var leastProductivePercent = sortedTeams.LastOrDefault()?.productive_percent ?? 0;

            var bottomTeams = sortedTeams
                .OrderBy(t => t.productive_percent).Take(3).ToList();

            var formattedTime = FormatDuration(GrandtotalProductiveDuration);
            var GrandTotalpercentage =
                                   (double)GrandtotalProductiveDuration / abc * 100 ;

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
                   COALESCE(SUM( DATEDIFF(SECOND, 0, TRY_CONVERT(TIME, a.Total_Time)) ), 0) AS PunchDuration
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
                    AND a.Total_Time IS NOT NULL 
                   AND A.AttendanceDate BETWEEN @FromDate AND @ToDate
                   AND (@TeamId IS NULL OR U.Id = @TeamId)
                   AND Us.Active = 1
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
                       AND Us.Active = 1
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
                var breakDuration = breakData?.break_duration ?? 0;

                // Convert the durations from seconds to TimeSpan
                var punchDurationTimeSpan = TimeSpan.FromSeconds(punchDuration);
                var breakDurationTimeSpan = TimeSpan.FromSeconds(breakDuration);
                var activeDurationTimeSpan = TimeSpan.FromSeconds(punchDuration - breakDuration);

                // Calculate total hours, minutes, and seconds, including overflow beyond 24 hours
                int totalPunchHours = (int)punchDurationTimeSpan.TotalHours;
                int punchMinutes = punchDurationTimeSpan.Minutes;
                int punchSeconds = punchDurationTimeSpan.Seconds;

                int totalBreakHours = (int)breakDurationTimeSpan.TotalHours;
                int breakMinutes = breakDurationTimeSpan.Minutes;
                int breakSeconds = breakDurationTimeSpan.Seconds;

                int totalActiveHours = (int)activeDurationTimeSpan.TotalHours;
                int activeMinutes = activeDurationTimeSpan.Minutes;
                int activeSeconds = activeDurationTimeSpan.Seconds;

                // Format the result as HH:mm:ss with total hours
                if (punchDuration > 0)
                {
                    return new
                    {
                        start_timing = date.ToString("yyyy-MM-dd"),
                        punch_duration = $"{totalPunchHours:D2}:{punchMinutes:D2}:{punchSeconds:D2}",
                        break_duration = $"{totalBreakHours:D2}:{breakMinutes:D2}:{breakSeconds:D2}",
                        active_duration = $"{totalActiveHours:D2}:{activeMinutes:D2}:{activeSeconds:D2}",
                    };
                }

                return null;
            }).Where(d => d != null).ToList();

            return new { data = mergedData };

        }
       
        public async Task<dynamic> GetProductivity_Trend(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                FromDate = fromDate,
                ToDate = toDate
            };

            // **Directly Execute the Stored Procedure Here (Instead of Calling GetAppUsagesS)**
            var usages = await _dapper.GetAllAsyncs<AppUsage>("Sp_GetAppUsagesS", parameters, commandType: CommandType.StoredProcedure);

            var dateWiseDurations = new List<DailyProductivityDuration>();
            var dailyDurations = new Dictionary<DateTime, DailyProductivityDuration>();

            foreach (var usage in usages)
            {
                usage.ApplicationName = usage.ApplicationName.ToLower();

                if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge")
                {
                    // **Query Category & Productivity Details**
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

                            // Fetch Productivity Name
                            var productivityQuery = @"
                    SELECT Name FROM ProductivityAssign
                    WHERE Id = @ProductivityId";

                            var productivityName = await _dapper.QueryFirstOrDefaultAsync<string>(productivityQuery, new { ProductivityId = category.ProductivityId });
                            usage.ProductivityName = productivityName;

                            var usageDate = usage.UsageDate.Date;

                            if (!dailyDurations.ContainsKey(usageDate))
                            {
                                dailyDurations[usageDate] = new DailyProductivityDuration
                                {
                                    Date = usageDate.ToString("yyyy-MM-dd")
                                };
                            }

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

                            dailyData.TotalDuration += usage.TotalSeconds;
                        }
                    }
                }
            }

            // **Aggregate Data by Date**
            var aggregatedDurations = dailyDurations.Values
                .Where(d => d.TotalDuration > 0)
                .OrderBy(d => d.Date)
                .ToList();

            foreach (var duration in aggregatedDurations)
            {
                string FormatDuration(long totalSeconds)
                {
                    var hours = totalSeconds / 3600;
                    var minutes = (totalSeconds % 3600) / 60;
                    var seconds = totalSeconds % 60;
                    return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
                }

                duration.Total_Duration = FormatDuration((long)Math.Round(duration.TotalDuration));
                duration.Productive_Duration = FormatDuration((long)Math.Round(duration.ProductiveDuration));
                duration.Unproductive_Duration = FormatDuration((long)Math.Round(duration.UnproductiveDuration));
                duration.Neutral_Duration = FormatDuration((long)Math.Round(duration.NeutralDuration));
            }

            return aggregatedDurations.Select(duration => new
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
    COALESCE(SUM( DATEDIFF(SECOND, 0, TRY_CONVERT(TIME, a.Total_Time)) ), 0) AS ActiveTimeInSeconds
FROM 
    Attendance A
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
    AND a.Total_Time IS NOT NULL      
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
    AND U.Active = 1
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
                         SELECT T.Id,T.Name
                          FROM Team T
                          INNER JOIN Organization O ON T.OrganizationId = O.Id
                          WHERE O.Id = @OrganizationId
                          AND (@TeamId IS NULL OR T.Id = @TeamId) ";

            var teams = await _dapper.GetAllAsync<(int TeamId, string TeamName)>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });

            var result = new List<dynamic>();

            if (userId.HasValue)
            {
                foreach (var team in teams)
                {
                    var TeamName = team.TeamName;
                    teamId = team.TeamId;

                    
                    var urlUsageQuery = "GetAppUsagesSS";
                    var parameters = new
                    {
                        OrganizationId = organizationId,
                        TeamId = teamId,
                        UserId = userId,
                        FromDate = fromDate,
                        ToDate = toDate
                    };

                    IEnumerable<dynamic> results = await _dapper.GetAllAsync<dynamic>(urlUsageQuery, parameters);
                    var getUsers = @"
                SELECT id AS UserId, CONCAT(First_Name, ' ', Last_Name) AS FullName 
                FROM Users 
                WHERE TeamId = @TeamId
                AND Active = 1 -- Filter only active users
                AND (@UserId IS NULL OR Id = @UserId)";

                    var getUsers1 = await _dapper.GetAllAsync<dynamic>(getUsers, parameters);

                    var tt = results.Concat(getUsers1)
                        .GroupBy(item => item.UserId)
                        .Select(group =>
                        {
                            var usageEntry = results.FirstOrDefault(u => u.UserId == group.Key);
                            return usageEntry ?? group.First();
                        })
                        .ToList();

                    foreach (var us in tt)
                    {
                        var userIdd = us.UserId;
                        var FullANme = us.FullName;
                        int totalProductiveDuration = 0;
                        int totalUnproductiveDuration = 0;
                        int totalNeutralDuration = 0;
                        var parameterss = new
                        {
                            OrganizationId = organizationId,
                            TeamId = teamId,
                            UserId = userId,
                            FromDate = fromDate,
                            ToDate = toDate
                        };
                        var usages = await _dapper.GetAllAsyncs<AppUsage>("Sp_GetAppUsageEmployeeList", parameterss, commandType: CommandType.StoredProcedure);


                        var totalUsages = usages
                        .GroupBy(u => u.ApplicationName)
                        .Select(g => new { ApplicationName = g.Key, TotalSeconds = g.Sum(u => u.TotalSeconds) })
                        .ToDictionary(t => t.ApplicationName, t => t.TotalSeconds);

                        foreach (var usage in usages)
                        {
                            usage.ApplicationName = usage.ApplicationName.ToLower();

                            if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge")
                            {
                                if (totalUsages.TryGetValue(usage.ApplicationName, out var totalSeconds))
                                {
                                    usage.TotalSeconds = totalSeconds;
                                    usage.TotalUsage = TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");
                                }
                                var parameters1 = new { ApplicationName = usage.ApplicationName.ToLower() };
                                var app = await _dapper.QueryFirstOrDefaultAsync<string>("GetApplicationCategoryAndProductivity",
                                    parameters1,
                                    commandType: CommandType.StoredProcedure
                                );
                                if (app != null)
                                {

                                    switch (app)
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
                        var AttendanceCount = us.AttendanceCount;
                        var onlineDurationInSeconds = us.OnlineDurationInHours ?? 0.0; // Ensure no null value
                        double? total_wokingtimeInSeconds = us.ActiveTimeInSeconds;  // Nullable double

                        total_wokingtimeInSeconds = us.ActiveTimeInSeconds ?? 0.0;
                        var breakDurationInSeconds = us.TotalBreakDurationInSeconds ?? 0.0;  // Ensure no null value
                        var totalDurationInSeconds = totalProductiveDuration + totalUnproductiveDuration + totalNeutralDuration;

                        var dateDifferenceInDays = (toDate - fromDate).TotalDays;

                        if (dateDifferenceInDays <= 0)
                        {
                            dateDifferenceInDays = 1;
                        }

                        // Ensure no null values are passed to FormatDuration
                        var percentageProductiveDuration = total_wokingtimeInSeconds > 0 ? ((double)totalProductiveDuration / total_wokingtimeInSeconds.Value) * 100 : 0.0;

                        // Helper function to format durations
                        string FormatDuration(double totalSeconds)
                        {
                            var hours = (long)(totalSeconds / 3600); // Total hours
                            var minutes = (long)((totalSeconds % 3600) / 60); // Remaining minutes
                            var seconds = (long)(totalSeconds % 60); // Remaining seconds
                            return $"{hours:D2}:{minutes:D2}:{seconds:D2}"; // Format as "HH:mm:ss"
                        }

                        // Create a dynamic object
                        var dynamicItem = new ExpandoObject() as dynamic;

                        dynamicItem.UserID = userIdd;
                        dynamicItem.full_Name = FullANme;
                        dynamicItem.Team_Name = TeamName;
                        dynamicItem.AttendanceCount = AttendanceCount;

                        // Format durations using the helper function
                        dynamicItem.total_wokingtime = FormatDuration(total_wokingtimeInSeconds ?? 0.0);
                        dynamicItem.BreakDuration = FormatDuration(breakDurationInSeconds);
                        dynamicItem.OnlineDuration = FormatDuration(onlineDurationInSeconds);

                        dynamicItem.TotalProductiveDuration = FormatDuration(totalProductiveDuration);
                        dynamicItem.TotalUnproductiveDuration = FormatDuration(totalUnproductiveDuration);
                        dynamicItem.TotalNeutralDuration = FormatDuration(totalNeutralDuration);
                        dynamicItem.TotalDuration = FormatDuration(totalDurationInSeconds);
                        dynamicItem.PercentageProductiveDuration = percentageProductiveDuration;

                        // Add the dynamic item to the result
                        result.Add(dynamicItem);
                    }


                }
                var orderedResult = result.OrderByDescending(us => (double)us.PercentageProductiveDuration).ToList();

                return orderedResult;
            }
            else
            {
                foreach (var team in teams)
                {

                    var TeamName = team.TeamName;
                    teamId = team.TeamId;
                    //var usagess = await GetAppUsagesSS(organizationId, teamId, userId, fromDate, toDate);
                    var urlUsageQuery = "GetAppUsagesSS";
                    var parameters = new
                    {
                        OrganizationId = organizationId,
                        TeamId = teamId,
                        UserId = userId,
                        FromDate = fromDate,
                        ToDate = toDate
                    };

                    IEnumerable<dynamic> results = await _dapper.GetAllAsync<dynamic>(urlUsageQuery, parameters);
                    var getUsers = @"
                SELECT id AS UserId, CONCAT(First_Name, ' ', Last_Name) AS FullName 
                FROM Users 
                WHERE TeamId = @TeamId
                AND Active = 1 -- Filter only active users
                AND (@UserId IS NULL OR Id = @UserId)";

                    var getUsers1 = await _dapper.GetAllAsync<dynamic>(getUsers, parameters);

                    var tt = results.Concat(getUsers1)
                        .GroupBy(item => item.UserId)
                        .Select(group =>
                        {
                            var usageEntry = results.FirstOrDefault(u => u.UserId == group.Key);
                            return usageEntry ?? group.First();
                        })
                        .ToList();


                    foreach (var us in tt)
                    {
                        userId = us.UserId;
                        var FullANme = us.FullName;

                        int totalProductiveDuration = 0, totalUnproductiveDuration = 0, totalNeutralDuration = 0;

                        var parameterss = new
                        {
                            OrganizationId = organizationId,
                            TeamId = teamId,
                            UserId = userId,
                            FromDate = fromDate,
                            ToDate = toDate
                        };
                        var usages = await _dapper.GetAllAsyncs<AppUsage>("Sp_GetAppUsageEmployeeList", parameterss, commandType: CommandType.StoredProcedure);


                        var totalUsages = usages
                        .GroupBy(u => u.ApplicationName)
                        .Select(g => new { ApplicationName = g.Key, TotalSeconds = g.Sum(u => u.TotalSeconds) })
                        .ToDictionary(t => t.ApplicationName, t => t.TotalSeconds);

                        foreach (var usage in usages)
                        {
                            usage.ApplicationName = usage.ApplicationName.ToLower();

                            if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge")
                            {
                                if (totalUsages.TryGetValue(usage.ApplicationName, out var totalSeconds))
                                {
                                    usage.TotalSeconds = totalSeconds;
                                    usage.TotalUsage = TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");
                                }

                                var parameters1 = new { ApplicationName = usage.ApplicationName.ToLower() };
                                var app = await _dapper.QueryFirstOrDefaultAsync<string>("GetApplicationCategoryAndProductivity",
                                    parameters1,
                                    commandType: CommandType.StoredProcedure
                                );
                                if (app != null)
                                {
                                    switch (app)
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
                        var AttendanceCount = us.AttendanceCount;
                        var onlineDurationInSeconds = us.OnlineDurationInHours ?? 0.0; // Ensure no null value
                        double? total_wokingtimeInSeconds = us.ActiveTimeInSeconds;  // Nullable double

                        total_wokingtimeInSeconds = us.ActiveTimeInSeconds ?? 0.0;
                        var breakDurationInSeconds = us.TotalBreakDurationInSeconds ?? 0.0;  // Ensure no null value
                        var totalDurationInSeconds = totalProductiveDuration + totalUnproductiveDuration + totalNeutralDuration;

                        var dateDifferenceInDays = (toDate - fromDate).TotalDays;

                        if (dateDifferenceInDays <= 0)
                        {
                            dateDifferenceInDays = 1;
                        }

                        // Ensure no null values are passed to FormatDuration
                        var percentageProductiveDuration = total_wokingtimeInSeconds > 0 ? ((double)totalProductiveDuration / total_wokingtimeInSeconds.Value) * 100 : 0.0; // Check before dividing

                        // Helper function to format durations
                        string FormatDuration(double totalSeconds)
                        {
                            var hours = (long)(totalSeconds / 3600); // Total hours
                            var minutes = (long)((totalSeconds % 3600) / 60); // Remaining minutes
                            var seconds = (long)(totalSeconds % 60); // Remaining seconds
                            return $"{hours:D2}:{minutes:D2}:{seconds:D2}"; // Format as "HH:mm:ss"
                        }

                        var dynamicItem = new ExpandoObject() as dynamic;

                        dynamicItem.UserID = userId;
                        dynamicItem.Team_Name = TeamName;
                        dynamicItem.full_Name = FullANme;
                        dynamicItem.AttendanceCount = AttendanceCount;

                        // Format durations using the helper function, ensure no null values
                        dynamicItem.total_wokingtime = FormatDuration(total_wokingtimeInSeconds ?? 0.0);
                        dynamicItem.BreakDuration = FormatDuration(breakDurationInSeconds);
                        dynamicItem.OnlineDuration = FormatDuration(onlineDurationInSeconds);

                        dynamicItem.TotalProductiveDuration = FormatDuration(totalProductiveDuration);
                        dynamicItem.TotalUnproductiveDuration = FormatDuration(totalUnproductiveDuration);
                        dynamicItem.TotalNeutralDuration = FormatDuration(totalNeutralDuration);
                        dynamicItem.TotalDuration = FormatDuration(totalDurationInSeconds);
                        dynamicItem.PercentageProductiveDuration = percentageProductiveDuration;

                        result.Add(dynamicItem);

                    }

                    userId = null;
                }
                var orderedResult = result.OrderByDescending(us => (double)us.PercentageProductiveDuration).ToList();

                return orderedResult;

            }

        }




    }
}

