using Hublog.Repository.Entities.Model.DashboardModel;

namespace Hublog.Service.Interface
{
    public interface IAttendanceDashboardService
    {
        Task<List<UserAttendanceReport>> GetUserTotalAttendanceAndBreakSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate);

        Task<List<AllAttendanceSummary>> GetAllAttendanceSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate); 
    }
}
