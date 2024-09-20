using Dapper;
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

        public async Task<AttendanceDashboardSummaryModel> AttendanceDashboardSummary(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                StartDate = startDate,
                EndDate = endDate
            };

            string query = "AttendanceDashboardSummary"; 

            return await _dapper.GetSingleAsync<AttendanceDashboardSummaryModel>(query, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<TeamProductivityModel>> GetTopTeamProductivity(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            string query = "GetTopTeamProductivity";

            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                StartDate = startDate,
                EndDate = endDate
            };

            return await _dapper.GetAllAsyncs<TeamProductivityModel>(query, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<TeamProductivityModel>> GetLeastTeamProductivity(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            var query = "GetBottomTeamProductivity";

            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                StartDate = startDate,
                EndDate = endDate
            };

            return await _dapper.GetAllAsyncs<TeamProductivityModel>(query, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<LateArrivalsModel>> GetLateArrivals(int organizationId, int? teamId, DateTime startDate, DateTime endDate)
        {
            string query = "LateArrivals";

            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                StartDate = startDate,
                EndDate = endDate
            };

            return await _dapper.GetAllAsyncs<LateArrivalsModel>(query, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
