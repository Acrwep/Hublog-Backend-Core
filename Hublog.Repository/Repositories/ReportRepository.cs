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
        public async Task<List<AttendanceReport>> AttendanceReport(int userId, int organizationId, DateTime date)
        {
            var query = @"
                          SELECT 
                            U.First_Name,
                            A.Start_Time,
                            A.End_Time,
                            A.AttendanceDate,
                            O.Id AS OrganizationId
                          FROM Users U 
                            INNER JOIN Attendance A ON U.Id = A.UserId
                            INNER JOIN Organization O ON A.OrganizationId = O.Id
                          WHERE U.Id = @UserId 
                            AND A.AttendanceDate = @AttendanceDate 
                            AND O.Id = @OrganizationId";

            return await _dapper.GetAllAsync<AttendanceReport>(query, new
            {
                UserId = userId,
                OrganizationId = organizationId,
                AttendanceDate = date
            });
        }
        #endregion
    }
}
