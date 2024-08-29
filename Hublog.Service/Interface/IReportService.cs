using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface IReportService
    {
        Task<List<AttendanceReport>> AttendanceReport(int? userId, int? teamId, int organizationId, DateTime date);   
    }
}
