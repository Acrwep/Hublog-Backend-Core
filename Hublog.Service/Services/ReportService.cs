using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }
        public async Task<List<AttendanceReport>> AttendanceReport(int? userId, int organizationId, DateTime date)
        {
            return await _reportRepository.AttendanceReport(userId, organizationId, date);  
        }
    }
}
