using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.DashboardModel;

namespace Hublog.Repository.Interface
{
    public interface IAttendanceDashboardRepository
    {
        Task<List<UserAttendanceReport>> GetUserTotalAttendanceAndBreakSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate);

        Task<List<AllAttendanceSummary>> GetAllAttendanceSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate); 
    }
}
