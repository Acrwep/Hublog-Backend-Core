using Dapper;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.DashboardModel;
using Hublog.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Hublog.Repository.Repositories
{
    public class AttendanceDashboardRepository : IAttendanceDashboardRepository
    {
        private readonly Dapperr _dapper;
        public AttendanceDashboardRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        public async Task<object> GetAllAttendanceSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate)
        {
            var sp = "GetAllAttendanceSummary";
            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            };

            var attendanceSummaries = await _dapper.GetAllAsyncs<AllAttendanceSummary>(sp, parameters, commandType: CommandType.StoredProcedure);

            int totalHours = 0;
            int totalMinutes = 0;
            int totalSeconds = 0;
            double totalPresentCount = 0;
            double totalAbsentCount = 0;

            foreach (var summary in attendanceSummaries)
            {

                if (!string.IsNullOrEmpty(summary.TotalWorkingTime))
                {
                    var timeParts = summary.TotalWorkingTime.Split(':');
                    if (timeParts.Length == 3)
                    {
                        int hours = int.Parse(timeParts[0]);
                        int minutes = int.Parse(timeParts[1]);
                        int seconds = int.Parse(timeParts[2]);

                        totalHours += hours;
                        totalMinutes += minutes;
                        totalSeconds += seconds;
                    }
                }

                totalPresentCount += summary.PresentCount;
                totalAbsentCount += summary.AbsentCount;
            }

            totalMinutes += totalSeconds / 60;
            totalSeconds = totalSeconds % 60;
            totalHours += totalMinutes / 60;
            totalMinutes = totalMinutes % 60;

            string overallTotalTimeFormatted = $"{totalHours}:{totalMinutes:D2}:{totalSeconds:D2}";

            double overallAttendancePercentage = 0;
            if (totalPresentCount + totalAbsentCount > 0)
            {
                overallAttendancePercentage = (totalPresentCount / (totalPresentCount + totalAbsentCount)) * 100;
            }
            return new
            {
                attendanceSummaries,
                overallTotalTime = overallTotalTimeFormatted,
                overallAttendancePercentage
            };
        }

        public async Task<List<UserAttendanceReport>> GetUserTotalAttendanceAndBreakSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate)
        {
            var sp = "GetUserTotalAttendanceAndBreakSummary";

            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            };

            return await _dapper.GetAllAsyncs<UserAttendanceReport>(sp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<AttendanceDashboardSummaryModel> AttendanceDashboardSummary(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                StartDate = startDate,
                EndDate = endDate
            };

            string query = "AttendanceDashboardSummary";

            return await _dapper.GetSingleAsync<AttendanceDashboardSummaryModel>(query, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<object> BreakTrends(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                StartDate = startDate,
                EndDate = endDate
            };

            string query = "GetBreakDurationByDate";

            var breakDurations = await _dapper.GetAllAsync<dynamic>(query, parameters);

            int totalHours = 0;
            int totalMinutes = 0;
            int totalSeconds = 0;

            foreach (var b in breakDurations)
            {
                var timeParts = b.BreakDuration.ToString().Split(':');
                if (timeParts.Length == 3)
                {
                    int hours = int.Parse(timeParts[0]);
                    int minutes = int.Parse(timeParts[1]);
                    int seconds = int.Parse(timeParts[2]);

                    totalHours += hours;
                    totalMinutes += minutes;
                    totalSeconds += seconds;
                }
            }

            totalMinutes += totalSeconds / 60;
            totalSeconds = totalSeconds % 60;
            totalHours += totalMinutes / 60;
            totalMinutes = totalMinutes % 60;

            string totalBreakDuration = $"{totalHours}:{totalMinutes:D2}:{totalSeconds:D2}";

            var result = breakDurations.Select(b => new
            {
                BreakDate = b.BreakDate.ToString("yyyy-MM-dd"),
                BreakDuration = b.BreakDuration
            }).ToList();

            return new
            {
                data = result,
                totalBreakDuration
            };
        }


        public async Task<List<TeamProductivityModel>> GetTopTeamProductivity(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            string query = "GetTopTeamProductivity";

            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                StartDate = startDate,
                EndDate = endDate
            };

            return await _dapper.GetAllAsyncs<TeamProductivityModel>(query, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<TeamProductivityModel>> GetLeastTeamProductivity(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            var query = "GetBottomTeamProductivity";

            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                StartDate = startDate,
                EndDate = endDate
            };

            return await _dapper.GetAllAsyncs<TeamProductivityModel>(query, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<object> GetLateArrivals(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            string query = "LateArrivals";

            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                StartDate = startDate,
                EndDate = endDate
            };

            var result = await _dapper.GetAllAsyncs<LateArrivalsModel>(query, parameters, commandType: CommandType.StoredProcedure);
            int totalLateArrivalsSum = result.Sum(r => r.LateArrival);
            int totalOnTimeArrivalsSum = result.Sum(r => r.OnTimeArrival);
            int totalAttendance = totalLateArrivalsSum + totalOnTimeArrivalsSum;

            double overallLatePercentage = totalAttendance > 0
                ? (double)totalLateArrivalsSum / totalAttendance * 100
                : 0; // Prevent division by zero


            return new
            {
                data = result,
                overallLatePercentage = overallLatePercentage
            };
        }
    }
}
