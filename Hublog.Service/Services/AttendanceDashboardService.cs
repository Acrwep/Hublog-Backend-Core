using Hublog.Repository.Entities.Model.DashboardModel;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class AttendanceDashboardService : IAttendanceDashboardService
    {
        private readonly IAttendanceDashboardRepository _attendanceDashboardRepository;
        public AttendanceDashboardService(IAttendanceDashboardRepository attendanceDashboardRepository) 
        {
            _attendanceDashboardRepository = attendanceDashboardRepository;
        }

        public async Task<List<AllAttendanceSummary>> GetAllAttendanceSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate) 
        {
            return await _attendanceDashboardRepository.GetAllAttendanceSummary(organizationId, teamId, userId, startDate, endDate);   
        }

        public async Task<List<UserAttendanceReport>> GetUserTotalAttendanceAndBreakSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate)
        {
            return await _attendanceDashboardRepository.GetUserTotalAttendanceAndBreakSummary(organizationId, teamId, userId, startDate, endDate);
        }
    }
}
