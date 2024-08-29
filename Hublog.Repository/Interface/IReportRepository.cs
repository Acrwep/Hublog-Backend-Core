using Hublog.Repository.Entities.Model;

namespace Hublog.Repository.Interface
{
    public interface IReportRepository
    {
        Task<List<AttendanceReport>> AttendanceReport(int? userId, int? teamId, int organizationId, DateTime date);   
    }
}
