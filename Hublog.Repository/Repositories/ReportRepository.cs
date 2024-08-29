using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;

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
            var query = @"
                          SELECT 
                            U.First_Name AS Employee,
                            A.Start_Time AS InTime,
                            A.End_Time AS Out,
                            A.Total_Time AS TotalTime,
                            A.AttendanceDate,
                            O.Id AS OrganizationId,
                            U.TeamId
                          FROM Users U 
                            INNER JOIN Attendance A ON U.Id = A.UserId
                            INNER JOIN Organization O ON A.OrganizationId = O.Id
                          WHERE (@UserId IS NULL OR U.Id = @UserId)
                            AND A.AttendanceDate = @AttendanceDate 
                            AND O.Id = @OrganizationId
                            AND(@TeamId IS NULL OR U.TeamId = @TeamId)";

            return await _dapper.GetAllAsync<AttendanceReport>(query, new
            {
                UserId = userId,
                OrganizationId = organizationId,
                AttendanceDate = date,
                TeamId = teamId
            });
        }
        #endregion
    }
}
