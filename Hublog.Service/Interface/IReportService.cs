using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Service.Interface
{
    public interface IReportService
    {
        Task<List<AttendanceReport>> AttendanceReport(int? userId, int? teamId, int organizationId, DateTime date);

        Task<List<BreaksReport>> BreakReport(int? userId, int? teamId, int organizationId, DateTime date);

        Task<List<AttedndanceLog>> GetMonthlyAttendanceReport(int? userId, int? teamId, int organizationId, int year, int month);

        Task<List<InOutLogs>> GetMonthlyInOutReport(int? userId, int? teamId, int organizationId, int year, int month);

        Task<List<CombinedUsageDto>> GetCombinedUsageReport(int organizationId, int? teamId, int? userId, string type, DateTime startDate, DateTime endDate);
        Task<List<dynamic>> DynamicReport([FromQuery] DynamicReportRequest request);
        Task<List<dynamic>> DynamicDetailReport([FromQuery] DynamicReportRequest request);
    }
}
