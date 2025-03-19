using Hublog.Repository.Entities.Model.DashboardModel;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Service.Services
{
    public class AttendanceDashboardService : IAttendanceDashboardService
    {
        private readonly IAttendanceDashboardRepository _attendanceDashboardRepository;
        public AttendanceDashboardService(IAttendanceDashboardRepository attendanceDashboardRepository) 
        {
            _attendanceDashboardRepository = attendanceDashboardRepository;
        }

        public async Task<object> GetAllAttendanceSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate) 
        {
            return await _attendanceDashboardRepository.GetAllAttendanceSummary(organizationId, teamId, userId, startDate, endDate);   
        }

        public async Task<List<UserAttendanceReport>> GetUserTotalAttendanceAndBreakSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate)
        {
            return await _attendanceDashboardRepository.GetUserTotalAttendanceAndBreakSummary(organizationId, teamId, userId, startDate, endDate);
        }

        public async Task<AttendanceDashboardSummaryModel> AttendanceDashboardSummary(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            return await _attendanceDashboardRepository.AttendanceDashboardSummary(organizationId, teamId, startDate, endDate);
        }

        public async Task<object> BreakTrends([FromQuery] int organizationId, [FromQuery] int? teamId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            return await _attendanceDashboardRepository.BreakTrends(organizationId, teamId, startDate, endDate);
        }

        public async Task<List<TeamProductivityModel>> GetTopTeamProductivity(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            return await _attendanceDashboardRepository.GetTopTeamProductivity(organizationId, teamId, startDate, endDate);
        }

        public async Task<List<TeamProductivityModel>> GetLeastTeamProductivity(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            return await _attendanceDashboardRepository.GetLeastTeamProductivity(organizationId,teamId, startDate, endDate);
        }

        public async Task<object> GetLateArrivals(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            return await _attendanceDashboardRepository.GetLateArrivals(organizationId, teamId, startDate, endDate);
        }
    }
}
