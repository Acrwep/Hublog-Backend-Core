﻿using Hublog.Repository.Common;
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

           var repor= await _dapper.GetAllAsyncs<AttedndanceLog>(query, parameter, commandType: CommandType.StoredProcedure);
            return repor;
        }
        #endregion

        #region GetMonthlyInOutReport
        public async Task<List<InOutLogs>> GetMonthlyInOutReport(int? userId, int? teamId, int organizationId, int year, int month)
        {
            var query = "GetMonthlyInOutReport";

            var parameter = new
            {
                TeamId = teamId,
                UserId=userId,
                OrganizationId = organizationId,
                Year = year,
                Month = month
            };
            var repor = await _dapper.GetAllAsyncs<InOutLogs>(query, parameter, commandType: CommandType.StoredProcedure);
            return repor;
        }
        #endregion

        public string FormatDuration(long totalSeconds)
        {
            var hours = totalSeconds / 3600; // Total hours
            var minutes = (totalSeconds % 3600) / 60; // Remaining minutes
            var seconds = totalSeconds % 60; // Remaining seconds
            return $"{hours:D2}:{minutes:D2}:{seconds:D2}"; // Format as "HH:mm:ss"
        }
        public async Task<List<CombinedUsageDto>> GetCombinedUsageReport(int organizationId, int? teamId, int? userId, string type, DateTime startDate, DateTime endDate)
        {
            var query = "Apps_Url_reports";

            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                Type=type,
                startDate = startDate,
                endDate = endDate
            };

            var result = await _dapper.GetAllAsync<CombinedUsageDto>(query, parameters);

            return result;
        }

    }
}
