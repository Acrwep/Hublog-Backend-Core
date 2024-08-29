using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using System.Data;

namespace Hublog.Repository.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly Dapperr _dapper;

        public ReportRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        #region AttendanceReport
        public async Task<List<AttendanceReport>> AttendanceReport(int? userId, int? teamId, int organizationId, DateTime date)
        {
            var query = "GetAttendanceReport";

            var parameters = new
            {
                UserId = userId,
                AttendanceDate = date,
                OrganizationId = organizationId,
                TeamId = teamId
            };

            return await _dapper.GetAllAsyncs<AttendanceReport>(query, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

        #region BreakReport
        public async Task<List<BreaksReport>> BreakReport(int? userId, int? teamId, int organizationId, DateTime date)
        {
            var query = "GetBreakReport"; 

            var parameters = new
            {
                UserId = userId,
                BreakDate = date,
                OrganizationId = organizationId,
                TeamId = teamId
            };

            return await _dapper.GetAllAsyncs<BreaksReport>(query, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion
    }
}
