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
                .TakeLast(3)
                .Select(u => new { UserId = u.Key, FullName = u.Value.FullName, AchievedDays = u.Value.AchievedDays })
                .ToList();

            return new { top = topAchievers, least = leastAchievers };
        }

    }
}
