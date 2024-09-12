using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.DashboardModel;
using Hublog.Repository.Interface;
using System.Data;

namespace Hublog.Repository.Repositories
{
    public class AttendanceDashboardRepository : IAttendanceDashboardRepository
    {
        private readonly Dapperr _dapper;
        public AttendanceDashboardRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        public async Task<List<AllAttendanceSummary>> GetAllAttendanceSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate) 
        {
            var sp = "GetAllAttendanceSummary";
            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            };

            return await _dapper.GetAllAsyncs<AllAttendanceSummary>(sp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<UserAttendanceReport>> GetUserTotalAttendanceAndBreakSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate) 
        {
            var sp = "GetUserTotalAttendanceAndBreakSummary";

            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            };

            return await _dapper.GetAllAsyncs<UserAttendanceReport>(sp, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
