using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.Goals;
using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Repository.Entities.Model.Shift;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Entities.Model.WellNess_Model;
using Hublog.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Hublog.Repository.Repositories
{
    public class GoalRepository : IGoalRepository
    {

        private readonly Dapperr _dapper;

        public GoalRepository(Dapperr dapper)
        {
            _dapper = dapper;

        }

        #region GetGoals
        public async Task<List<Goal>> GetGoals(int organizationId)
        {
            try
            {
                var query = @"SELECT * FROM Goals WHERE OrganizationId = @OrganizationId";

                var parameters = new
                {
                    OrganizationId = organizationId
                };

                return await _dapper.GetAllAsync<Goal>(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Goals record", ex);
            }
        }
        #endregion


        #region InsertGoals  
        public async Task<Goal> InsertGoals(Goal goal)
        {
            const string query = @"INSERT INTO Goals (OrganizationId, WorkingTime, ProductiveTime)
                VALUES (@OrganizationId, @WorkingTime, @ProductiveTime);
                SELECT CAST(SCOPE_IDENTITY() as int)";


            var createdGoals = await _dapper.ExecuteAsync(query, goal);
            goal.Id = createdGoals;
            return goal;
        }
        #endregion


        #region GetGoals
        public async Task<Goal> UpdateGoals(Goal goal)
        {
            try
            {
                string query = @" UPDATE Goals 
                                  SET OrganizationId = @OrganizationId, WorkingTime = @WorkingTime, ProductiveTime = @ProductiveTime
                                  WHERE Id = @Id"
                ;

                var result = await _dapper.ExecuteAsync(query, goal);

                if (result > 0)
                {
                    string selectQuery = @"
                                           SELECT Id, OrganizationId, WorkingTime, ProductiveTime
                                           FROM Goals
                                           WHERE Id = @Id"
                    ;
                    var getUpdatedValue = await _dapper.GetAsync<Goal>(selectQuery, new { Id = goal.Id });
                    return getUpdatedValue;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating Goals", ex);
            }
        }
        #endregion

        //  public async Task<dynamic> GetGoalsDetails(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        //  {

        //      var teamQuery = @"
        //                   SELECT T.Id,T.Name
        //                    FROM Team T
        //                    INNER JOIN Organization O ON T.OrganizationId = O.Id
        //                    WHERE O.Id = @OrganizationId
        //                    AND (@TeamId IS NULL OR T.Id = @TeamId) ";

        //      var teams = await _dapper.GetAllAsync<(int TeamId, string TeamName)>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });

        //      var result = new List<dynamic>();

        //      foreach (var team in teams)
        //      {

        //          var TeamName = team.TeamName;
        //          teamId = team.TeamId;
        //          //var usagess = await GetAppUsagesSS(organizationId, teamId, userId, fromDate, toDate);
        //          var urlUsageQuery = "GetGoalsDetails";
        //          var parameters = new
        //          {
        //              OrganizationId = organizationId,
        //              TeamId = teamId,
        //              FromDate = fromDate,
        //              ToDate = toDate
        //          };

        //          IEnumerable<dynamic> results = await _dapper.GetAllAsync<dynamic>(urlUsageQuery, parameters);
        //          var getUsers = @"
        //          SELECT id AS UserId, CONCAT(First_Name, ' ', Last_Name) AS FullName 
        //          FROM Users 
        //          WHERE TeamId = @TeamId
        //          AND Active = 1 -- Filter only active users";

        //          var getUsers1 = await _dapper.GetAllAsync<dynamic>(getUsers, parameters);

        //          var tt = results.Concat(getUsers1)
        //              .GroupBy(item => item.UserId)
        //              .Select(group =>
        //              {
        //                  var usageEntry = results.FirstOrDefault(u => u.UserId == group.Key);
        //                  return usageEntry ?? group.First();
        //              })
        //              .ToList();

        //          int? userId;
        //          foreach (var us in tt)
        //          {
        //              var FullANme = us.FullName;
        //              userId = us.UserId;

        //              int totalProductiveDuration = 0;

        //              var usages = await GetAppUsages(organizationId, teamId, userId, fromDate, toDate);

        //              var totalUsages = usages
        //              .GroupBy(u => u.ApplicationName)
        //              .Select(g => new { ApplicationName = g.Key, TotalSeconds = g.Sum(u => u.TotalSeconds) })
        //              .ToDictionary(t => t.ApplicationName, t => t.TotalSeconds);

        //              foreach (var usage in usages)
        //              {
        //                  usage.ApplicationName = usage.ApplicationName.ToLower();

        //                  if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge")
        //                  {
        //                      if (totalUsages.TryGetValue(usage.ApplicationName, out var totalSeconds))
        //                      {
        //                          usage.TotalSeconds = totalSeconds;
        //                          usage.TotalUsage = TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");
        //                      }

        //                      var parameters1 = new { ApplicationName = usage.ApplicationName.ToLower() };
        //                      var app = await _dapper.QueryFirstOrDefaultAsync<string>("GetApplicationCategoryAndProductivity",
        //                          parameters1,
        //                          commandType: CommandType.StoredProcedure
        //                      );
        //                      if (app != null)
        //                      {
        //                          if (app == "Productive")
        //                          {
        //                              totalProductiveDuration += usage.TotalSeconds;
        //                          }
        //                      }
        //                  }
        //              }
        //              double? total_wokingtimeInSeconds = us.ActiveTimeInSeconds;  // Nullable double

        //              total_wokingtimeInSeconds = us.ActiveTimeInSeconds ?? 0.0;

        //              var dateDifferenceInDays = (toDate - fromDate).TotalDays;

        //              if (dateDifferenceInDays <= 0)
        //              {
        //                  dateDifferenceInDays = 1;
        //              }

        //              // Helper function to format durations
        //              string FormatDuration(double totalSeconds)
        //              {
        //                  var hours = (long)(totalSeconds / 3600); // Total hours
        //                  var minutes = (long)((totalSeconds % 3600) / 60); // Remaining minutes
        //                  var seconds = (long)(totalSeconds % 60); // Remaining seconds
        //                  return $"{hours:D2}:{minutes:D2}:{seconds:D2}"; // Format as "HH:mm:ss"
        //              }

        //              var dynamicItem = new ExpandoObject() as dynamic;

        //              dynamicItem.UserId = userId;
        //              dynamicItem.Team_Name = TeamName;
        //              dynamicItem.full_Name = FullANme;

        //              // Format durations using the helper function, ensure no null values
        //              dynamicItem.total_wokingtime = FormatDuration(total_wokingtimeInSeconds ?? 0.0);

        //              dynamicItem.TotalProductiveDuration = FormatDuration(totalProductiveDuration);

        //              result.Add(dynamicItem);
        //          }
        //          userId = null;
        //      }
        //      var getGoalsQuery = "SELECT * FROM Goals WHERE OrganizationId = @OrganizationId";
        //      var GoalsData = await _dapper.QueryFirstOrDefaultAsync<Goal>(getGoalsQuery, new { OrganizationId = organizationId });
        //      Console.WriteLine(GoalsData);

        //      int workingTimeThreshold = GoalsData?.WorkingTime ?? 0;
        //      int productiveTimeThreshold = GoalsData?.ProductiveTime ?? 0;


        //      // Convert HH:mm:ss to total hours
        //      Func<string, double> ConvertToHours = (time) =>
        //      {
        //          TimeSpan ts;
        //          if (TimeSpan.TryParseExact(time, @"hh\:mm\:ss", CultureInfo.InvariantCulture, out ts))
        //          {
        //              return ts.TotalHours;
        //          }
        //          return 0;
        //      };

        //      var filteredUsers = result
        //.Where(user =>
        //    ConvertToHours(user.total_wokingtime) > workingTimeThreshold &&
        //    ConvertToHours(user.TotalProductiveDuration) > productiveTimeThreshold)
        //.ToList();

        //      // Get top 3 users (highest productive time, then working time)
        //      var topUsers = filteredUsers
        //          .OrderByDescending(user => ConvertToHours(user.TotalProductiveDuration))
        //          .ThenByDescending(user => ConvertToHours(user.total_wokingtime))
        //          .Take(3)
        //          .ToList();

        //      // Get users who did not meet the threshold condition
        //      var nonAchievedUsers = result
        //          .Where(user =>
        //              ConvertToHours(user.total_wokingtime) <= workingTimeThreshold ||
        //              ConvertToHours(user.TotalProductiveDuration) <= productiveTimeThreshold)
        //          .ToList();

        //      // Get least 3 users (who did not achieve the condition, sorted by lowest productivity first)
        //      var leastUsers = nonAchievedUsers
        //          .OrderBy(user => ConvertToHours(user.TotalProductiveDuration))  // Lowest productive first
        //          .ThenBy(user => ConvertToHours(user.total_wokingtime))  // Then lowest working time
        //          .Take(3)
        //          .ToList();

        //      // Combine top and least users into a dictionary or an anonymous object
        //      var resultData = new
        //      {
        //          top = topUsers,
        //          least = leastUsers
        //      };

        //      Console.WriteLine(resultData);

        //      return resultData;

        //  }






        //public async Task<dynamic> GetGoalsDetails(int organizationId, int? teamId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        //{
        //    var getGoalsQuery = "SELECT * FROM Goals WHERE OrganizationId = @OrganizationId";
        //    var GoalsData = await _dapper.QueryFirstOrDefaultAsync<Goal>(getGoalsQuery, new { OrganizationId = organizationId });

        //    double workingTimeThreshold = GoalsData?.WorkingTime ?? 0;  // Hours
        //    double productiveTimeThreshold = GoalsData?.ProductiveTime ?? 0;  // Hours

        //    var teamQuery = @"
        //SELECT T.Id, T.Name
        //FROM Team T
        //INNER JOIN Organization O ON T.OrganizationId = O.Id
        //WHERE O.Id = @OrganizationId
        //AND (@TeamId IS NULL OR T.Id = @TeamId)";

        //    var teams = await _dapper.GetAllAsync<(int TeamId, string TeamName)>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });

        //    var userAchievementCounts = new Dictionary<int, (string FullName, int AchievedDays, double TotalWorkingHours, double TotalProductiveHours)>();

        //    foreach (var team in teams)
        //    {
        //        var TeamName = team.TeamName;
        //        teamId = team.TeamId;

        //        var parameters = new
        //        {
        //            OrganizationId = organizationId,
        //            TeamId = teamId,
        //            FromDate = fromDate,
        //            ToDate = toDate
        //        };

        //        IEnumerable<dynamic> results = await _dapper.GetAllAsync<dynamic>("GetGoalsDetails", parameters);

        //        var getUsersQuery = @"
        //    SELECT id AS UserId, CONCAT(First_Name, ' ', Last_Name) AS FullName 
        //    FROM Users 
        //    WHERE TeamId = @TeamId AND Active = 1";

        //        var getUsers = await _dapper.GetAllAsync<dynamic>(getUsersQuery, parameters);

        //        var usersList = results.Concat(getUsers)
        //            .GroupBy(item => item.UserId)
        //            .Select(group =>
        //            {
        //                var usageEntry = results.FirstOrDefault(u => u.UserId == group.Key);
        //                return usageEntry ?? group.First();
        //            })
        //            .ToList();

        //        foreach (var user in usersList)
        //        {
        //            var fullName = user.FullName;
        //            int userId = user.UserId;

        //            int achievedCount = 0;
        //            double totalWorkingHours = 0.0;
        //            double totalProductiveHours = 0.0;

        //            for (DateTime date = fromDate; date <= toDate; date = date.AddDays(1))
        //            {
        //                int totalProductiveDurationInSeconds = 0;
        //                double totalWorkingTimeInSeconds = 0.0;

        //                var usages = await GetAppUsages(organizationId, teamId, userId, date, date);

        //                var totalUsages = usages
        //                    .GroupBy(u => u.ApplicationName)
        //                    .Select(g => new { ApplicationName = g.Key, TotalSeconds = g.Sum(u => u.TotalSeconds) })
        //                    .ToDictionary(t => t.ApplicationName, t => t.TotalSeconds);

        //                foreach (var usage in usages)
        //                {
        //                    usage.ApplicationName = usage.ApplicationName.ToLower();

        //                    if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge")
        //                    {
        //                        if (totalUsages.TryGetValue(usage.ApplicationName, out var totalSeconds))
        //                        {
        //                            usage.TotalSeconds = totalSeconds;
        //                        }

        //                        var parameters1 = new { ApplicationName = usage.ApplicationName.ToLower() };
        //                        var app = await _dapper.QueryFirstOrDefaultAsync<string>("GetApplicationCategoryAndProductivity",
        //                            parameters1,
        //                            commandType: CommandType.StoredProcedure
        //                        );

        //                        if (app != null && app == "Productive")
        //                        {
        //                            totalProductiveDurationInSeconds += usage.TotalSeconds;
        //                        }
        //                    }
        //                }

        //                totalWorkingTimeInSeconds = results
        //                    .Where(u => u.UserId == userId && ((DateTime)u.AttendanceDate).Date == date.Date)
        //                    .Select(u => (double?)u.ActiveTimeInSeconds)
        //                    .FirstOrDefault() ?? 0.0;

        //                // ** Convert seconds to hours **
        //                double totalWorkingTimeInHours = totalWorkingTimeInSeconds / 3600.0;
        //                double totalProductiveDurationInHours = totalProductiveDurationInSeconds / 3600.0;

        //                totalWorkingHours += totalWorkingTimeInHours;
        //                totalProductiveHours += totalProductiveDurationInHours;

        //                // Check if the user meets the goals in hours
        //                if (totalWorkingTimeInHours >= workingTimeThreshold && totalProductiveDurationInHours >= productiveTimeThreshold)
        //                {
        //                    achievedCount++;
        //                }
        //            }

        //            userAchievementCounts[userId] = (fullName, achievedCount, totalWorkingHours, totalProductiveHours);
        //        }
        //    }
        //    // ** Get Top 3 Achievers (Only users who achieved at least 1 day) **
        //    var topAchievers = userAchievementCounts
        //        .Where(u => u.Value.AchievedDays > 0)  // Only users who achieved at least once
        //        .OrderByDescending(u => u.Value.AchievedDays)
        //        .Take(3)
        //        .Select(u => new
        //        {
        //            UserId = u.Key,
        //            FullName = u.Value.FullName,
        //            AchievedDays = u.Value.AchievedDays
        //        })
        //        .ToList();

        //    // ** Get Bottom 3 from Users who NEVER achieved the goal**
        //    var leastAchievers = userAchievementCounts
        //        .Where(u => u.Value.AchievedDays == 0)  // Users who never achieved the goal
        //        .OrderBy(u => u.Value.TotalWorkingHours)  // Sort by lowest working hours
        //        .ThenBy(u => u.Value.TotalProductiveHours)  // Then by lowest productive hours
        //        .Take(3)
        //        .Select(u => new
        //        {
        //            UserId = u.Key,
        //            FullName = u.Value.FullName,
        //            TotalWorkingHours = u.Value.TotalWorkingHours,
        //            TotalProductiveHours = u.Value.TotalProductiveHours
        //        })
        //        .ToList();

        //    return new { top = topAchievers, least = leastAchievers };
        //}



        public async Task<dynamic> GetGoalsDetails(int organizationId, int? teamId, DateTime fromDate, DateTime toDate)
        {
            var getGoalsQuery = "SELECT * FROM Goals WHERE OrganizationId = @OrganizationId";
            var goalsData = await _dapper.QueryFirstOrDefaultAsync<Goal>(getGoalsQuery, new { OrganizationId = organizationId });

            double workingTimeThreshold = goalsData?.WorkingTime ?? 0;
            double productiveTimeThreshold = goalsData?.ProductiveTime ?? 0;

            var teamQuery = @"
    SELECT T.Id, T.Name
    FROM Team T
    INNER JOIN Organization O ON T.OrganizationId = O.Id
    WHERE O.Id = @OrganizationId
    AND (@TeamId IS NULL OR T.Id = @TeamId)";

            var teams = await _dapper.GetAllAsync<(int TeamId, string TeamName)>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });

            var userAchievementCounts = new Dictionary<int, (string FullName, int AchievedDays, double TotalWorkingHours, double TotalProductiveHours)>();

            foreach (var team in teams)
            {
                var teamName = team.TeamName;
                teamId = team.TeamId;

                var parameters = new { OrganizationId = organizationId, TeamId = teamId, FromDate = fromDate, ToDate = toDate };

                var results = await _dapper.GetAllAsyncs<dynamic>("GetGoalsDetails", parameters, commandType: CommandType.StoredProcedure);

                var usersQuery = @"SELECT id AS UserId, CONCAT(First_Name, ' ', Last_Name) AS FullName FROM Users WHERE TeamId = @TeamId AND Active = 1";
                var users = await _dapper.GetAllAsync<dynamic>(usersQuery, parameters);

                var usersList = results.Concat(users)
                    .GroupBy(item => item.UserId)
                    .Select(group => results.FirstOrDefault(u => u.UserId == group.Key) ?? group.First())
                    .ToList();

                foreach (var user in usersList)
                {
                    var fullName = user.FullName;
                    int userId = user.UserId;
                    int achievedCount = 0;
                    double totalWorkingHours = 0.0;
                    double totalProductiveHours = 0.0;

                    for (DateTime date = fromDate; date <= toDate; date = date.AddDays(1))
                    {
                        int totalProductiveDurationInSeconds = 0;
                        double totalWorkingTimeInSeconds = 0.0;

                        var usageParameters = new { OrganizationId = organizationId, TeamId = teamId, UserId = userId, FromDate = date, ToDate = date };
                        var usages = await _dapper.GetAllAsyncs<AppUsage>("GetCombinedUsage", usageParameters, commandType: CommandType.StoredProcedure);

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
                                }
                                var app = await _dapper.QueryFirstOrDefaultAsync<string>("GetApplicationCategoryAndProductivity", new { ApplicationName = usage.ApplicationName }, commandType: CommandType.StoredProcedure);
                                if (app != null && app == "Productive")
                                {
                                    totalProductiveDurationInSeconds += usage.TotalSeconds;
                                }
                            }
                        }

                        totalWorkingTimeInSeconds = results
                            .Where(u => u.UserId == userId && ((DateTime)u.AttendanceDate).Date == date.Date)
                            .Select(u => (double?)u.ActiveTimeInSeconds)
                            .FirstOrDefault() ?? 0.0;

                        double totalWorkingTimeInHours = totalWorkingTimeInSeconds / 3600.0;
                        double totalProductiveDurationInHours = totalProductiveDurationInSeconds / 3600.0;

                        totalWorkingHours += totalWorkingTimeInHours;
                        totalProductiveHours += totalProductiveDurationInHours;

                        if (totalWorkingTimeInHours >= workingTimeThreshold && totalProductiveDurationInHours >= productiveTimeThreshold)
                        {
                            achievedCount++;
                        }
                    }
                    userAchievementCounts[userId] = (fullName, achievedCount, totalWorkingHours, totalProductiveHours);
                }
            }

            var topAchievers = userAchievementCounts
                .Where(u => u.Value.AchievedDays > 0)
                .OrderByDescending(u => u.Value.AchievedDays)
                .Take(3)
                .Select(u => new { UserId = u.Key, FullName = u.Value.FullName, AchievedDays = u.Value.AchievedDays })
                .ToList();

            var leastAchievers = userAchievementCounts
                .Where(u => u.Value.AchievedDays == 0)
                .OrderBy(u => u.Value.TotalWorkingHours)
                .ThenBy(u => u.Value.TotalProductiveHours)
                .Take(3)
                .Select(u => new { UserId = u.Key, FullName = u.Value.FullName, TotalWorkingHours = u.Value.TotalWorkingHours, TotalProductiveHours = u.Value.TotalProductiveHours })
                .ToList();

            return new { top = topAchievers, least = leastAchievers };
        }




        //public async Task<List<AppUsage>> GetAppUsages(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        //{
        //    string appUsageQuery = @"
        //           SELECT 
        //               A.UserId, 
        //               A.ApplicationName, 
        //               A.Details, 
        // SUM(
        //        -- Convert TotalUsage into total seconds manually
        //        CAST(SUBSTRING(A.TotalUsage, 1, CHARINDEX(':', A.TotalUsage) - 1) AS INT) * 3600 +  -- Hours to seconds
        //        CAST(SUBSTRING(A.TotalUsage, CHARINDEX(':', A.TotalUsage) + 1, CHARINDEX(':', A.TotalUsage, CHARINDEX(':', A.TotalUsage) + 1) - CHARINDEX(':', A.TotalUsage) - 1) AS INT) * 60 +  -- Minutes to seconds
        //        CAST(SUBSTRING(A.TotalUsage, CHARINDEX(':', A.TotalUsage, CHARINDEX(':', A.TotalUsage) + 1) + 1, LEN(A.TotalUsage)) AS INT)  -- Seconds
        //    ) AS TotalSeconds, 
        //               A.UsageDate
        //           FROM  
        //               ApplicationUsage A
        //           INNER JOIN 
        //               Users U ON A.UserId = U.Id
        //           INNER JOIN 
        //                   Team T ON T.Id = U.TeamId
        //           INNER JOIN 
        //               Organization O ON U.OrganizationId = O.Id
        //           WHERE  
        //                  O.Id = @OrganizationId 
        //                  AND A.UsageDate BETWEEN @FromDate AND @ToDate
        //                  AND (@TeamId IS NULL OR T.Id = @TeamId)
        //                  AND (@UserId IS NULL OR A.UserId = @UserId)
        //                  AND U.Active = 1
        //           GROUP BY 
        //               A.UserId, 
        //               A.ApplicationName, 
        //               A.Details, 
        //               A.UsageDate;
        //        ";

        //    var urlUsageQuery = @"
        //    SELECT 
        //        U.UserId,
        //        U.Url AS ApplicationName,
        //        NULL AS Details,
        // SUM(
        //        CAST(SUBSTRING( U.TotalUsage, 1, CHARINDEX(':', U.TotalUsage) - 1) AS INT) * 3600 +  -- Hours to seconds
        //        CAST(SUBSTRING( U.TotalUsage, CHARINDEX(':',  U.TotalUsage) + 1, CHARINDEX(':',  U.TotalUsage, CHARINDEX(':', U.TotalUsage) + 1) - CHARINDEX(':',  U.TotalUsage) - 1) AS INT) * 60 +  -- Minutes to seconds
        //        CAST(SUBSTRING( U.TotalUsage, CHARINDEX(':', U.TotalUsage, CHARINDEX(':',  U.TotalUsage) + 1) + 1, LEN( U.TotalUsage)) AS INT)  -- Seconds
        //    ) AS TotalSeconds, 
        //        U.UsageDate
        //    FROM 
        //        UrlUsage U
        //    INNER JOIN 
        //        Users Us ON U.UserId = Us.Id
        //    INNER JOIN 
        //        Team T ON T.Id = Us.TeamId
        //    INNER JOIN 
        //        Organization O ON Us.OrganizationId = O.Id
        //    WHERE 
        //        O.Id = @OrganizationId 
        //        AND U.UsageDate BETWEEN @FromDate AND @ToDate
        //        AND (@TeamId IS NULL OR T.Id = @TeamId)
        //        AND (@UserId IS NULL OR U.UserId = @UserId)
        //        AND Us.Active = 1
        //    GROUP BY 
        //        U.UserId,
        //        U.Url,
        //        U.UsageDate;
        //";
        //    ;
        //    var parameters = new
        //    {
        //        OrganizationId = organizationId,
        //        TeamId = teamId,
        //        UserId = userId,
        //        FromDate = fromDate,
        //        ToDate = toDate
        //    };

        //    var appUsages = await _dapper.GetAllAsync<AppUsage>(appUsageQuery, parameters);
        //    var urlUsages = await _dapper.GetAllAsync<AppUsage>(urlUsageQuery, parameters);

        //    var allUsages = appUsages.Concat(urlUsages);

        //    // Group by ApplicationName (URL or Application) to handle duplicates
        //    var groupedUsages = allUsages
        //        .GroupBy(u => u.ApplicationName.ToLower()) // Grouping by URL in lowercase for case-insensitivity
        //        .Select(g => new AppUsage
        //        {
        //            UserId = g.First().UserId, // You can choose how to handle UserId if there are multiple
        //            ApplicationName = g.Key, // Use the grouped ApplicationName
        //            Details = g.First().Details, // Keep the first Details (adjust as needed)
        //            TotalSeconds = g.Sum(u => u.TotalSeconds), // Sum the TotalSeconds for the same URL
        //            UsageDate = g.Min(u => u.UsageDate) // Optional: Use earliest date as the representative date
        //        })
        //        .ToList();

        //    return groupedUsages;
        //}

    }
}
