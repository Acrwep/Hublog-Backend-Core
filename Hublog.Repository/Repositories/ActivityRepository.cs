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

                // Update totals
                totalActiveDuration += activeTime;
                totalIdealDuration += idleTime;

                // Prepare breakdown for each team
                breakdown.Add(new Activity_Duration
                {
                    TeamName = team.Name,
                    ActiveTime = activeTime,
                    IdleDuration = idleTime,
                    Duration = activeTime + idleTime
                });


            }
            var sortedTeams = breakdown
        .Select(team => new
        {
            TeamName = team.TeamName,
            ActiveTime = team.ActiveTime ?? 0,
            IdleDuration = team.IdleDuration ?? 0,
            TotalDuration = (team.ActiveTime ?? 0) + (team.IdleDuration ?? 0),
            ActiveTimePercent = ((team.ActiveTime ?? 0) + (team.IdleDuration ?? 0)) == 0
                ? 0 // Avoid division by zero
                : ((team.ActiveTime ?? 0) * 100.0) / ((team.ActiveTime ?? 0) + (team.IdleDuration ?? 0))
        })
        .OrderByDescending(team => team.ActiveTimePercent)
        .ToList();

            var topTeams = sortedTeams .Where(team => team.ActiveTimePercent > 0).Take(3) .ToList();
            var bottomTeams = sortedTeams.OrderBy(team => team.ActiveTimePercent).Take(3).ToList();


            // Calculate overall totals and percentage
            double totalDuration = totalActiveDuration + totalIdealDuration;
            double totalActiveTimePer = (totalDuration == 0) ? 0 : (totalActiveDuration / totalDuration) * 100;
            var dateDifferenceInDays = (toDate - fromDate).TotalDays;
            dateDifferenceInDays++;
            var averageDurationInSeconds = totalActiveDuration / dateDifferenceInDays;

            // Prepare final result
            var result = new
            {
                data = new
                {
                    total_active_time = FormatDuration(totalActiveDuration),
                    total_active_time_per = totalActiveTimePer,
                    total_idle_duration = FormatDuration(totalIdealDuration),
                    AverageDuratiopn = FormatDuration((long)averageDurationInSeconds)
                },
                teams = breakdown.Select(team => new
                {
                    team_name = team.TeamName,
                    active_time = FormatDuration((long)(team.ActiveTime ?? 0)), // Convert to long
                    idle_duration = FormatDuration((long)(team.IdleDuration ?? 0)), // Convert to long
                    duration = FormatDuration((long)((team.ActiveTime ?? 0) + (team.IdleDuration ?? 0))) // Convert to long
                }).ToList(),
                top = topTeams.Select(team => new
                {
                    ActiveTime = FormatDuration(team.ActiveTime),
                    IdleDuration = FormatDuration(team.IdleDuration),
                    total_duration = FormatDuration(team.TotalDuration),
                    ActiveTimePercent = double.IsFinite(team.ActiveTimePercent) ? team.ActiveTimePercent : 0, // Validate percentage
                    team_name = team.TeamName
                }).ToList(),
                bottom = bottomTeams.Select(team => new
                {
                    ActiveTime = FormatDuration(team.ActiveTime),
                    IdleDuration = FormatDuration(team.IdleDuration),
                    total_duration = FormatDuration(team.TotalDuration),
                    ActiveTimePercent = double.IsFinite(team.ActiveTimePercent) ? team.ActiveTimePercent : 0, // Validate percentage
                    team_name = team.TeamName
                }).ToList()
            };

            return result;
        }
        public async Task<dynamic> MostLeast_Teamwise_Activity(int organizationId, int? teamId, DateTime fromDate, DateTime toDate)
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
                        total_active_time = 0,
                        total_idle_duration = 0,
                        total_duration = 0,
                        total_active_time_per = 0.0,
                        team_name = "N/A"
                    }
                },
                        bottom = new[]
                        {

                    new
                    {
                        total_active_time = 0,
                        total_idle_duration = 0,
                        total_duration = 0,
                        total_active_time_per = 0.0,
                        team_name = "N/A"
                    }
                }
                    }
                };
            }

            var teamResults = new List<dynamic>();
            var GrandtotalTimeDuration = 0;
            var GrandtotalProductiveDuration = 0;
            long abc = 0;
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
                IEnumerable<Activity_Duration> usages = await _dapper.GetAllAsync<Activity_Duration>(urlUsageQuery, parameters);

            }
            return teamResults;
        }

    }
}
