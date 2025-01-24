using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Hublog.Repository.Entities.Model.Activity;
using Hublog.Repository.Entities.Model.UserModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Hublog.Repository.Entities.Model.WellNess_Model;
using System.Globalization;
using System.Data;

namespace Hublog.Repository.Repositories
{
    public class WellnessRepository : IWellnessRepository
    {
        private readonly Dapperr _dapper;
        public WellnessRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }
        public string FormatDuration(long totalSeconds)
        {
            var hours = totalSeconds / 3600; // Total hours
            var minutes = (totalSeconds % 3600) / 60; // Remaining minutes
            var seconds = totalSeconds % 60; // Remaining seconds
            return $"{hours:D2}:{minutes:D2}:{seconds:D2}"; // Format as "HH:mm:ss"
        }
        public async Task<ResultModel> InsertWellness(List<UserBreakModel> userBreakModels)
        {
            try
            {
                var formattedDetails = new List<dynamic>();
                foreach (var item in userBreakModels)
                {
                    formattedDetails.Add(new
                    {
                        item.OrganizationId,
                        item.BreakEntryId,
                        item.Id,
                        item.UserId,
                        BreakDate = item.BreakDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        Start_Time = item.Start_Time.ToString("yyyy-MM-dd HH:mm:ss"),
                        End_Time = item.End_Time?.ToString("yyyy-MM-dd HH:mm:ss"),
                        item.Status
                    });
                }

                string details = JsonConvert.SerializeObject(formattedDetails);

                var parameters = new { details };
                var result = await _dapper.GetAsync<ResultModel>("Exec [SP_BreakEntry] @details", parameters);

                Console.WriteLine($"Stored Procedure Result Message: {result.Msg}");

                if (result != null && result.Msg.Contains("ongoing break"))
                {
                    Console.WriteLine("Entering ongoing break condition.");

                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting break: {ex.Message}");
                return new ResultModel { Result = 0, Msg = ex.Message };
            }
        }

        public async Task<object> GetWellness([FromQuery] int OrganizationId)
        {
            var parameters = new
            {
                OrganizationId = OrganizationId
            };
            var teamQuery = @"
                           SELECT W.Healthy, W.Overburdened,W.Underutilized
                           FROM wellness W
                           INNER JOIN Organization O ON W.OrganizationId = O.Id
                           WHERE O.Id = @OrganizationId ";

            var teams = await _dapper.GetAllAsync<WellNess>(teamQuery, new { OrganizationId = OrganizationId });
            return teams;
        }
        public async Task<WellNess> UpdateWellNess(int organizationId, WellNess wellNess)
        {
            if (wellNess == null)
                throw new ArgumentNullException(nameof(wellNess), "The wellness object cannot be null.");

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object>
    {
        { "OrganizationId", organizationId }
    };

            if (wellNess.Underutilized.HasValue)
            {
                setClauses.Add("Underutilized = @Underutilized");
                parameters.Add("Underutilized", wellNess.Underutilized);
            }

            if (wellNess.Healthy.HasValue)
            {
                setClauses.Add("Healthy = @Healthy");
                parameters.Add("Healthy", wellNess.Healthy);
            }

            if (wellNess.Overburdened.HasValue)
            {
                setClauses.Add("Overburdened = @Overburdened");
                parameters.Add("Overburdened", wellNess.Overburdened);
            }

            if (setClauses.Count == 0)
                throw new Exception("No valid values to update.");

            var query = $@"
        UPDATE wellness
        SET {string.Join(", ", setClauses)}
        WHERE OrganizationId = @OrganizationId";

            var rowsAffected = await _dapper.ExecuteAsync(query, parameters);

            if (rowsAffected == 0)
                throw new Exception("No wellness data found or updated for the given organization.");

            var fetchQuery = @"
        SELECT 
            Id, 
            OrganizationId, 
            Underutilized, 
            Healthy, 
            Overburdened
        FROM wellness
        WHERE OrganizationId = @OrganizationId";

            var updatedWellNess = await _dapper.QueryFirstOrDefaultAsync<WellNess>(fetchQuery, new { OrganizationId = organizationId });

            if (updatedWellNess == null)
                throw new Exception("Failed to fetch updated wellness data.");

            return updatedWellNess;
        }

        public async Task<object> GetWellnessSummaryPreviousDate(int organizationId, int? teamId, [FromQuery] DateTime date)
        {
            var teamQuery = @"
                    SELECT T.Id, T.Name
                    FROM Team T
                    INNER JOIN Organization O ON T.OrganizationId = O.Id
                    WHERE O.Id = @OrganizationId
                    AND (@TeamId IS NULL OR T.Id = @TeamId)";

            var teams = await _dapper.GetAllAsync<Team>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });

            int TotalactiveTimeSec = 0;
            int totalHealthySec = 0;

            foreach (var team in teams)
            {
                var parameters = new
                {
                    OrganizationId = organizationId,
                    TeamId = team.Id,
                    Date = date
                };

                var sp = "EXEC GetWellnessSummary @OrganizationId, @TeamId, @Date";
                var activityDurations = await _dapper.QueryAsync<TeamWiseUsers>(sp, parameters);

                var wellnessQuery = "SELECT * FROM Wellness WHERE OrganizationId = @OrganizationId";
                var wellnessData = await _dapper.QueryFirstOrDefaultAsync<WellNess>(wellnessQuery, new { OrganizationId = organizationId });

                int healthyThreshold = wellnessData?.Healthy ?? 0;
                int underutilizedThreshold = wellnessData?.Underutilized ?? 0;

                foreach (var user in activityDurations)
                {
                    int activeTimeSec = user?.ActiveTime ?? 0;
                    TotalactiveTimeSec += activeTimeSec;

                    int activeTime = activeTimeSec / 3600;
                    if (activeTime >= underutilizedThreshold && activeTime < healthyThreshold)
                    {
                        totalHealthySec += activeTimeSec;
                    }
                }
            }

            double HealthyPercentage = TotalactiveTimeSec == 0 ? 0 : ((double)totalHealthySec / TotalactiveTimeSec) * 100;
            int previousDateTotalactiveTimeSec = TotalactiveTimeSec == 0 ? 0 : TotalactiveTimeSec;

            return new { PreviousdateHealthyPercentage = HealthyPercentage, PreviousDateTotalactiveTimeSec = previousDateTotalactiveTimeSec };
        }


        public async Task<object> GetWellnessSummary(int organizationId, int? teamId, [FromQuery] DateTime date)
        {
            var teamQuery = @"
                            SELECT T.Id, T.Name
                            FROM Team T
                            INNER JOIN Organization O ON T.OrganizationId = O.Id
                            WHERE O.Id = @OrganizationId
                            AND (@TeamId IS NULL OR T.Id = @TeamId)";

            var teams = await _dapper.GetAllAsync<Team>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });

            var wellnessSummaries = new List<WellNess>();
            var wellnessHealthy = new List<dynamic>(); 
            var wellnessOverburdened = new List<dynamic>();
            var wellnessUnderutilized = new List<object>();

            int totalHealthyCount = 0, totalOverburdenedCount = 0, totalUnderutilizedCount = 0, TotalactiveTimeSec = 0;
            int totalHealthySec = 0, totalOverburdenedSec = 0, totalUnderutilizedSec = 0;

            int topHealthyUserId = 0, topHealthyTimeSec = 0;
            int topOverburdenedUserId = 0, topOverburdenedTimeSec = 0;
            string topHealthyFullName = "";
            string topOverburdenedFullName = "";

            var healthyUsers = new List<(int UserId, string FullName, int ActiveTimeSec)>();
            var overburdenedUsers = new List<(int UserId, string FullName, int ActiveTimeSec)>();
            var overUnderutilizedUsers = new List<(int UserId, string FullName, int ActiveTimeSec)>();


            foreach (var team in teams)
            {
                var parameters = new
                {
                    OrganizationId = organizationId,
                    TeamId = team.Id,
                    Date = date
                };

                var sp = "EXEC GetWellnessSummary @OrganizationId, @TeamId, @Date";
                var activityDurations = await _dapper.QueryAsync<TeamWiseUsers>(sp, parameters);

                var wellnessQuery = "SELECT * FROM Wellness WHERE OrganizationId = @OrganizationId";
                var wellnessData = await _dapper.QueryFirstOrDefaultAsync<WellNess>(wellnessQuery, new { OrganizationId = organizationId });

                int healthyThreshold = wellnessData?.Healthy ?? 0;
                int overburdenedThreshold = wellnessData?.Overburdened ?? 0;
                int underutilizedThreshold = wellnessData?.Underutilized ?? 0;

                int healthyCount = 0, overburdenedCount = 0, underutilizedCount = 0;

                foreach (var user in activityDurations)
                {
                    int userId = user?.UserId ?? 0;
                    string fullName = user.Fullname;  
                    int activeTimeSec = user?.ActiveTime ?? 0;
                    TotalactiveTimeSec += activeTimeSec;
                    int activeTime = activeTimeSec / 3600;

                    if (activeTime >= underutilizedThreshold && activeTime < healthyThreshold)
                    {
                        healthyCount++;
                        totalHealthySec += activeTimeSec;

                        healthyUsers.Add((userId, fullName, activeTimeSec));

                        if (activeTimeSec > topHealthyTimeSec)
                        {
                            topHealthyTimeSec = activeTimeSec;
                            topHealthyUserId = userId;
                            topHealthyFullName = fullName;
                        }
                    }
                    else if (activeTime >= healthyThreshold)
                    {
                        overburdenedCount++;
                        totalOverburdenedSec += activeTimeSec;

                        overburdenedUsers.Add((userId, fullName, activeTimeSec));

                        if (activeTimeSec > topOverburdenedTimeSec)
                        {
                            topOverburdenedTimeSec = activeTimeSec;
                            topOverburdenedUserId = userId;
                            topOverburdenedFullName = fullName;
                        }
                    }
                    else if (activeTime < underutilizedThreshold)
                    {
                        underutilizedCount++;
                        totalUnderutilizedSec += activeTimeSec;
                        overUnderutilizedUsers.Add((userId, fullName, activeTimeSec));

                    }
                }

                totalHealthyCount += healthyCount;
                totalOverburdenedCount += overburdenedCount;
                totalUnderutilizedCount += underutilizedCount;

                wellnessSummaries.Add(new WellNess
                {
                    TeamId = team.Id,
                    TeamName = team.Name,
                    Healthy = healthyCount,
                    Overburdened = overburdenedCount,
                    Underutilized = underutilizedCount
                });
            }

            var topHealthyUsers = wellnessSummaries.OrderByDescending(u => u.Healthy).Take(3).ToList();
            var topOverburdenedUsers = wellnessSummaries.OrderByDescending(u => u.Overburdened).Take(3).ToList();
            var topoverUnderutilizedUsers = wellnessSummaries.OrderByDescending(u => u.Underutilized).Take(3).ToList();

            wellnessHealthy = topHealthyUsers.Select(u => (dynamic)new { u.TeamName, u.TeamId, u.Healthy }).ToList();
            wellnessOverburdened = topOverburdenedUsers.Select(u => (dynamic)new { u.TeamName, u.TeamId, u.Overburdened }).ToList();
            wellnessUnderutilized = topoverUnderutilizedUsers.Select(u => (dynamic)new { u.TeamName, u.TeamId, u.Underutilized }).ToList();

            date = date.AddDays(-1);
            var result = await GetWellnessSummaryPreviousDate(organizationId, teamId, date);

            double PreviousdateHealthyPercentage = (double)((dynamic)result).PreviousdateHealthyPercentage;

            double CurrentHealthyPercentage = TotalactiveTimeSec == 0 ? 0 : ((double)totalHealthySec / TotalactiveTimeSec) * 100;

            double HealthyPercentageDifference = CurrentHealthyPercentage- PreviousdateHealthyPercentage;


            string total_active_time = FormatDuration(TotalactiveTimeSec);
            var previousDateTotalActiveTimeSec = (int)((dynamic)result).PreviousDateTotalactiveTimeSec;
            double percentageDifference = 0;
            if (previousDateTotalActiveTimeSec == 0)
            {
                if (TotalactiveTimeSec == 0)
                {
                    percentageDifference = previousDateTotalActiveTimeSec == 0 ? 0 : ((double)(TotalactiveTimeSec - previousDateTotalActiveTimeSec) / previousDateTotalActiveTimeSec) * 100;
                }
                else
                {
                    percentageDifference = 100;
                }
            }
            else
            {
                percentageDifference = previousDateTotalActiveTimeSec == 0 ? 0 : ((double)(TotalactiveTimeSec - previousDateTotalActiveTimeSec) / previousDateTotalActiveTimeSec) * 100;
            }
            string totalHealthy_time = FormatDuration(totalHealthySec);
            string totalOverburdened_time = FormatDuration(totalOverburdenedSec);
            string totalUnderutilized_time = FormatDuration(totalUnderutilizedSec);
            string topOverburdenedTime = FormatDuration(topOverburdenedTimeSec);
            string topHealthyTime = FormatDuration(topHealthyTimeSec);
            string HealthyComparison = HealthyPercentageDifference > 0
                ? "Greater Than"
                : (HealthyPercentageDifference < 0 ? "Less Than" : "No Variation");

            string WorkingTimeComparison = percentageDifference > 0
                ? "Greater Than"
                : (percentageDifference < 0 ? "Less Than" : "No Variation");
            return new
            {
                Healthyemployees = new
                {
                    HealthyemployeesPercentage = CurrentHealthyPercentage,
                    previousHealthyemployeesPercentage = HealthyPercentageDifference,
                    Comparison = HealthyComparison
                },
                Workingtime = new
                {
                    TotalWorkingtime = total_active_time,
                    TotalWorkingtimePercentage = percentageDifference,
                    Comparison = WorkingTimeComparison
                },
                TopOverburdenedemployee = new
                {
                    UserId = topOverburdenedUserId,
                    FullName = topOverburdenedFullName,
                    ActiveTimeSec = topOverburdenedTime
                },
                TopHealthyemployee = new
                {
                    UserId = topHealthyUserId,
                    FullName = topHealthyFullName,
                    ActiveTimeSec = topHealthyTime
                },
                OverallWellnessCount = new
                {
                    HealthyCount = totalHealthyCount,
                    OverburdenedCount = totalOverburdenedCount,
                    UnderutilizedCount = totalUnderutilizedCount
                },
                Top3WellnessHealthy = wellnessHealthy,
                Top3WellnessOverburdened = wellnessOverburdened,
                Top3WellnessUnderutilized = wellnessUnderutilized,
                WellnessSummaries = wellnessSummaries
            };
        }

        public async Task<object> GetWellnessDetails(int organizationId, int? teamId, int? userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var teamQuery = @"
        SELECT T.Id, T.Name 
        FROM Team T
        INNER JOIN Organization O ON T.OrganizationId = O.Id
        WHERE O.Id = @OrganizationId
        AND (@TeamId IS NULL OR T.Id = @TeamId)";

            var teams = await _dapper.GetAllAsync<(int TeamId, string TeamName)>(
                teamQuery,
                new { OrganizationId = organizationId, TeamId = teamId }
            );

            var dateWiseDurations = new List<WellNessBreakdown>();

            foreach (var team in teams)
            {
                var parameters = new
                {
                    OrganizationId = organizationId,
                    TeamId = team.TeamId,
                    UserId = userId,
                    StartDate = startDate,
                    EndDate = endDate
                };

                var usages = await _dapper.GetAllAsync<WellNessBreakdown>("GetWellnessTimeTrend", parameters);

                dateWiseDurations.AddRange(usages);
            }

            var wellnessQuery = "SELECT [Healthy], [Overburdened], [Underutilized] FROM Wellness WHERE OrganizationId = @OrganizationId";
            var wellnessData = await _dapper.QueryFirstOrDefaultAsync<WellNess>(
                wellnessQuery,
                new { OrganizationId = organizationId }
            );

            int healthyThreshold = wellnessData?.Healthy ?? 0;
            int overburdenedThreshold = wellnessData?.Overburdened ?? 0;
            int underutilizedThreshold = wellnessData?.Underutilized ?? 0;

            var datewiseCounts = dateWiseDurations
                .Where(u => u.Date != default)
                .GroupBy(u => u.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    HealthyCount = g.Count(u => (u.ActiveTime ?? 0) / 3600 < healthyThreshold && (u.ActiveTime ?? 0) / 3600 >= underutilizedThreshold),
                    OverburdenedCount = g.Count(u => (u.ActiveTime ?? 0) / 3600 >= healthyThreshold),
                    UnderutilizedCount = g.Count(u => (u.ActiveTime ?? 0) / 3600 < underutilizedThreshold)
                })
                .OrderBy(d => d.Date) 
                .ToList();

            return new
            {
                DatewiseWellnessCount = datewiseCounts
            };
        }

        public async Task<object> GetWellnessUserDetails(int organizationId, int? teamId, int? userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var urlUsageQuery = "userswise_Wellness";
            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                FromDate = startDate,
                ToDate = endDate
            };

            var result = await _dapper.GetAllAsync<WellnessUserDetails>(urlUsageQuery, parameters);

            var employees = result.Select(u => new
            {
                u.TeamId,
                u.TeamName,
                u.UserId,
                u.FullName,
                ActiveTime = u.TotalTime.HasValue ? TimeSpan.FromSeconds(u.TotalTime.Value).ToString(@"hh\:mm\:ss") : "00:00:00",
                AttendanceCount = u.TotalPresent, 
                Underutilized = u.Underutilized,
                Healthy = u.Healthy,
                Overburdened = u.Overburdened,
                UnderutilizedPercentage = u.UnderutilizedPercentage,
                HealthyPercentage = u.HealthyPercentage,
                OverburdenedPercentage = u.OverburdenedPercentage
            }).OrderByDescending(r => r.AttendanceCount).ToList();

            return new
            {
                Employees = employees
            };
        }

    }
}
