using Hublog.Repository.Common;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.UserModels;
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

        #region GetMonthlyAttendanceReport
        public async Task<List<AttedndanceLog>> GetMonthlyAttendanceReport(int? userId, int? teamId, int organizationId, int year, int month)
        {
            var query = "GetMonthlyAttendanceReport";

            var parameter = new
            {
                UserId = userId,
                TeamId = teamId,
                OrganizationId = organizationId,
                Year = year,
                Month = month
            };

            return await _dapper.GetAllAsyncs<AttedndanceLog>(query, parameter, commandType: CommandType.StoredProcedure);
        }
        #endregion

        #region GetMonthlyInOutReport
        public async Task<List<InOutLogs>> GetMonthlyInOutReport(int? userId, int? teamId, int organizationId, int year, int month)
        {
            var query = "GetMonthlyInOutReport";

            var parameter = new
            {
                TeamId = teamId,
                OrganizationId = organizationId,
                Year = year,
                Month = month
            };

            return await _dapper.GetAllAsyncs<InOutLogs>(query, parameter, commandType: CommandType.StoredProcedure);
        }
        #endregion


        public async Task<List<CombinedUsageDto>> GetCombinedUsageReport(int organizationId, int? teamId, int? userId, string type, DateTime startDate, DateTime endDate)
        {
            var query = @"
        WITH CombinedUsage AS (
            SELECT 
                'URL' AS Type,
                UrlUsage.Url AS Details,
                DATEDIFF(SECOND, 0, CAST(UrlUsage.TotalUsage AS TIME)) AS TotalUsage, 
                UrlUsage.UsageDate,
                Users.OrganizationId,
                Users.TeamId,
                Users.Id AS UserId
            FROM 
                UrlUsage
            JOIN 
                Users ON Users.Id = UrlUsage.UserId
            WHERE 
                UrlUsage.TotalUsage IS NOT NULL

            UNION ALL

            SELECT 
                'Application' AS Type,
                ApplicationUsage.ApplicationName AS Details,
                DATEDIFF(SECOND, 0, CAST(ApplicationUsage.TotalUsage AS TIME)) AS TotalUsage, 
                ApplicationUsage.UsageDate,
                Users.OrganizationId,
                Users.TeamId,
                Users.Id AS UserId
            FROM 
                ApplicationUsage
            JOIN 
                Users ON Users.Id = ApplicationUsage.UserId
            WHERE 
                ApplicationUsage.TotalUsage IS NOT NULL
        )
        SELECT
            Type,
            Details,
            CONVERT(VARCHAR(8), DATEADD(SECOND, SUM(TotalUsage), 0), 108) AS TotalUsage,
            SUM(TotalUsage) * 100.0 / NULLIF((SELECT SUM(TotalUsage) FROM CombinedUsage WHERE OrganizationId = @OrganizationId), 0) AS UsagePercentage
        FROM 
            CombinedUsage
        WHERE 
            OrganizationId = @OrganizationId
            AND (@TeamId IS NULL OR TeamId = @TeamId)
            AND (@UserId IS NULL OR UserId = @UserId)
            AND (@Type IS NULL OR Type = @Type)
            AND UsageDate BETWEEN @StartDate AND @EndDate
        GROUP BY
            Type,
            Details
        ORDER BY
            TotalUsage DESC;";

            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                Type = type,
                StartDate = startDate,
                EndDate = endDate
            };

            return await _dapper.GetAllAsync<CombinedUsageDto>(query, parameters);
        }

    }
}
