using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Activity;
using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Repository.Interface;

namespace Hublog.Repository.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly Dapperr _dapper;
        public ActivityRepository(Dapperr dapper)
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
        public async Task<object> GetActivityBreakDown(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            var teamQuery = @"
        SELECT T.Id, T.Name
        FROM Team T
        INNER JOIN Organization O ON T.OrganizationId = O.Id
        WHERE O.Id = @OrganizationId
        AND (@TeamId IS NULL OR T.Id = @TeamId)";

            var teams = await _dapper.GetAllAsync<Team>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });

            int totalActiveDuration = 0;
            int totalIdealDuration = 0;
            int totalBreakDuration = 0;
            int totalOnlineDuration = 0;
            int total_Duration = 0;
            List<Activity_Duration> breakdown = new List<Activity_Duration>();

            foreach (var team in teams)
            {
                var parameters = new
                {
                    OrganizationId = organizationId,
                    TeamId = team.Id,
                    UserId = userId,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                // Execute the stored procedure to get team data
                var sp = "EXEC GetTeamSummary @OrganizationId, @TeamId, @UserId, @FromDate, @ToDate";
                var breakMaster = await _dapper.QueryFirstOrDefaultAsync<Activity_Duration>(sp, parameters);

                // Handle null `breakMaster`
                int activeTime = breakMaster?.ActiveTime ?? 0;
                int idleTime = breakMaster?.IdleDuration ?? 0;
                int BreakTime = breakMaster?.BreakDuration ?? 0;
                int OnlineTime = breakMaster?.OnlineTime ?? 0;
                int WorkingTime = activeTime;

                // Update totals
                totalActiveDuration += activeTime-(idleTime + BreakTime);
                totalIdealDuration += idleTime;
                totalBreakDuration += BreakTime;
                totalOnlineDuration += activeTime-BreakTime;
                total_Duration += activeTime;
                OnlineTime = activeTime - BreakTime;
                activeTime -= (idleTime + BreakTime);

                // Prepare breakdown for each team
                breakdown.Add(new Activity_Duration
                {
                    TeamName = team.Name,
                    ActiveTime = activeTime,
                    IdleDuration = idleTime,
                    BreakDuration= BreakTime,
                    OnlineTime = OnlineTime,
                    Duration = WorkingTime
                });


            }
            var sortedTeams = breakdown
        .Select(team => new
        {
            TeamName = team.TeamName,
            ActiveTime = team.ActiveTime ?? 0,
            IdleDuration = team.IdleDuration ?? 0,
            BreakDuration = team.BreakDuration ?? 0,
            OnlineTime = team.OnlineTime ?? 0,
            TotalDuration = team.Duration ?? 0,
            ActiveTimePercent = team.Duration.HasValue && team.Duration > 0
        ? ((team.ActiveTime ?? 0) / (double)(team.Duration ?? 0)) * 100
        : 0
        })
        .OrderByDescending(team => team.ActiveTimePercent)
        .ToList();

            var topTeams = sortedTeams .Where(team => team.ActiveTimePercent > 0).Take(3) .ToList();
            var bottomTeams = sortedTeams.OrderBy(team => team.ActiveTimePercent).Take(3).ToList();


            // Calculate overall totals and percentage
            double totalDuration = totalOnlineDuration + totalIdealDuration;
            double totalActiveTimePer = (totalDuration == 0) ? 0 : (totalActiveDuration / totalDuration) * 100;
            var dateDifferenceInDays = (toDate - fromDate).TotalDays;
            dateDifferenceInDays++;
            var averageDurationInSeconds = totalOnlineDuration / dateDifferenceInDays;

            // Prepare final result
            var result = new
            {
                data = new
                {
                    total_active_time = FormatDuration(totalActiveDuration),
                    total_active_time_per = totalActiveTimePer,
                    total_idle_duration = FormatDuration(totalIdealDuration),
                    total_Online_Duration = FormatDuration(totalOnlineDuration),
                    total_Break_Duration = FormatDuration(totalBreakDuration),
                    total_Duration = FormatDuration(total_Duration),
                    AverageDuration = FormatDuration((long)averageDurationInSeconds)
                },
                teams = breakdown.Select(team => new
                {
                    team_name = team.TeamName,
                    active_time = FormatDuration((long)(team.ActiveTime ?? 0)), // Convert to long
                    idle_duration = FormatDuration((long)(team.IdleDuration ?? 0)), // Convert to long
                    BreakDuration = FormatDuration((long)(team.BreakDuration ?? 0)), // Convert to long
                    OnlineTime = FormatDuration((long)(team.OnlineTime ?? 0)), // Convert to long
                    duration = FormatDuration((long)(team.Duration ?? 0))// Convert to long
                }).ToList(),
                top = topTeams.Select(team => new
                {
                    ActiveTime = FormatDuration(team.ActiveTime),
                    IdleDuration = FormatDuration(team.IdleDuration),
                    OnlineTime = FormatDuration(team.OnlineTime),
                    total_duration = FormatDuration(team.TotalDuration),
                    ActiveTimePercent = double.IsFinite(team.ActiveTimePercent) ? team.ActiveTimePercent : 0, // Validate percentage
                    team_name = team.TeamName
                }).ToList(),
                bottom = bottomTeams.Select(team => new
                {
                    ActiveTime = FormatDuration(team.ActiveTime),
                    IdleDuration = FormatDuration(team.IdleDuration),
                    OnlineTime = FormatDuration(team.OnlineTime),
                    total_duration = FormatDuration(team.TotalDuration),
                    ActiveTimePercent = double.IsFinite(team.ActiveTimePercent) ? team.ActiveTimePercent : 0, // Validate percentage
                    team_name = team.TeamName
                }).ToList()
            };

            return result;
        }
        public async Task<dynamic> Date_wise_Activity(int organizationId, int? teamId,int? userid, DateTime fromDate, DateTime toDate)
        {
            // Query to fetch teams
            var teamQuery = @"
    SELECT T.Id, T.Name 
    FROM Team T
    INNER JOIN Organization O ON T.OrganizationId = O.Id
    WHERE O.Id = @OrganizationId
    AND (@TeamId IS NULL OR T.Id = @TeamId)";

            // Get list of teams
            var teams = await _dapper.GetAllAsync<(int TeamId, string TeamName)>(teamQuery, new { OrganizationId = organizationId, TeamId = teamId });
            var dateWiseDurations = new List<DailyActivityDuration>();

            foreach (var team in teams)
            {
                // Fetch usage data for each team
                var parameters = new
                {
                    OrganizationId = organizationId,
                    TeamId = team.TeamId,
                    UserId= userid,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                var usages = await _dapper.GetAllAsync<DailyActivityDuration>("Datewise_Activity", parameters);

                // Populate daily durations
                var dailyDurations = usages
                    .GroupBy(u => u.Date)
                    .ToDictionary(
                        g => g.Key,
                        g => new DailyActivityDuration
                        {
                            Date = g.Key,
                            OnlineTime = g.Sum(u => u.ActiveTime - u.BreakDuration),
                            IdleDuration = g.Sum(u => u.IdleDuration),
                            BreakDuration=g.Sum(u => u.BreakDuration),
                            ActiveDuration= g.Sum(u => u.ActiveTime - (u.IdleDuration + u.BreakDuration)),
                            TotalDuration = g.Sum(u => u.ActiveTime)
                        });

                dateWiseDurations.AddRange(dailyDurations.Values);
            }

            // Aggregate and format results
            var filteredDurations = dateWiseDurations
                .GroupBy(d => d.Date)
                .Select(g => new DailyActivityDuration
                {
                    Date = g.Key,
                    TotalDuration = g.Sum(d => d.TotalDuration),
                    OnlineTime = g.Sum(d => d.OnlineTime),
                    IdleDuration = g.Sum(d => d.IdleDuration),
                    BreakDuration=g.Sum(d => d.BreakDuration),
                    ActiveDuration=g.Sum(d=>d.ActiveDuration)
                })
                .Where(d => d.TotalDuration > 0)
                .OrderBy(d => d.Date)
                .ToList();

            // Format durations as "HH:mm:ss"
            foreach (var duration in filteredDurations)
            {
                duration.Total_Duration = FormatDuration((long)Math.Round(duration.TotalDuration));
                duration.Online_Time = FormatDuration((long)Math.Round(duration.OnlineTime));
                duration.Idle_Duration = FormatDuration((long)Math.Round(duration.IdleDuration));
                duration.Break_Duration = FormatDuration((long)Math.Round(duration.BreakDuration));
                duration.Active_Duration = FormatDuration((long)Math.Round(duration.ActiveDuration));
            }

            // Return formatted results
            return filteredDurations.Select(d => new
            {
                date = d.Date,
                total_Duration = d.Total_Duration,
                Online_Time = d.Online_Time,
                Idle_Duration = d.Idle_Duration,
                Break_Duration = d.Break_Duration,
                Active_Duration=d.Active_Duration
            }).ToList();
        }


    }
}
