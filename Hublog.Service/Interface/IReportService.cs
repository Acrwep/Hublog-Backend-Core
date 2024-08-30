using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;

namespace Hublog.Service.Interface
{
    public interface IReportService
    {
        Task<List<AttendanceReport>> AttendanceReport(int? userId, int? teamId, int organizationId, DateTime date);

        Task<List<BreaksReport>> BreakReport(int? userId, int? teamId, int organizationId, DateTime date);

        Task<List<AttedndanceLog>> GetMonthlyAttendanceReport(int? userId, int? teamId, int organizationId, int year, int month);
    }
}
