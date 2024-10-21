using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.DashboardModel;

namespace Hublog.Repository.Interface
{
    public interface IAttendanceDashboardRepository
    {
        Task<List<UserAttendanceReport>> GetUserTotalAttendanceAndBreakSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate);

        Task<List<AllAttendanceSummary>> GetAllAttendanceSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate);

        Task<AttendanceDashboardSummaryModel> AttendanceDashboardSummary(int organizationId, int? teamId, DateTime startDate, DateTime endDate);

        Task<List<TeamProductivityModel>> GetTopTeamProductivity(int organizationId, int? teamId, DateTime startDate, DateTime endDate);

        Task<List<TeamProductivityModel>> GetLeastTeamProductivity(int organizationId, int? teamId, DateTime startDate, DateTime endDate);

        Task<List<LateArrivalsModel>> GetLateArrivals(int organizationId, int? teamId, DateTime startDate, DateTime endDate);
    }
}
