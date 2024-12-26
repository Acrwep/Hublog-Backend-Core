using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Service.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }
        public async Task<List<AttendanceReport>> AttendanceReport(int? userId, int? teamId, int organizationId, DateTime date)
        {
            return await _reportRepository.AttendanceReport(userId, teamId, organizationId, date);  
        }

        public async Task<List<BreaksReport>> BreakReport(int? userId, int? teamId, int organizationId, DateTime date)
        {
            return await _reportRepository.BreakReport(userId, teamId, organizationId, date);
        }

        public async Task<List<AttedndanceLog>> GetMonthlyAttendanceReport(int? userId, int? teamId, int organizationId, int year, int month)
        {
            return await _reportRepository.GetMonthlyAttendanceReport(userId, teamId, organizationId, year, month);
        }

        public async Task<List<InOutLogs>> GetMonthlyInOutReport(int? userId, int? teamId, int organizationId, int year, int month)
        {
            return await _reportRepository.GetMonthlyInOutReport(userId, teamId, organizationId, year, month);
        }

        public async Task<List<CombinedUsageDto>> GetCombinedUsageReport(int organizationId, int? teamId, int? userId,string type, DateTime startDate, DateTime endDate)
        {
            return await _reportRepository.GetCombinedUsageReport(organizationId, teamId, userId, type, startDate, endDate);
        }
        public async Task<List<dynamic>> DynamicReport([FromQuery] DynamicReportRequest request)
        {
            return await _reportRepository.DynamicReport(request);
        }
        public async Task<List<dynamic>> DynamicDetailReport([FromQuery] DynamicReportRequest request)
        {
            return await _reportRepository.DynamicDetailReport(request);
        }
    }
}
