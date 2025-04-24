using Hublog.Repository.Common;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Dynamic;

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
            var query = "sp_GetAttendanceReport";

            var parameters = new
            {
                UserId = userId,
                AttendanceDate = date,
                OrganizationId = organizationId,
                TeamId = teamId
            };

            var kk = await _dapper.GetAllAsyncs<AttendanceReport>(query, parameters, commandType: CommandType.StoredProcedure);
            return kk;
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

            var repor = await _dapper.GetAllAsyncs<AttedndanceLog>(query, parameter, commandType: CommandType.StoredProcedure);
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
                UserId = userId,
                OrganizationId = organizationId,
                Year = year,
                Month = month
            };
            var repor = await _dapper.GetAllAsyncs<InOutLogs>(query, parameter, commandType: CommandType.StoredProcedure);
            return repor;
        }
        #endregion


        public async Task<object> GetLateAttendance(int organizationId, int? userId, int? teamId, DateTime date)
        {
            string query = "GetLateAttendance";

            var parameters = new
            {
                OrganizationId = organizationId,
                UserId = userId,
                TeamId = teamId,
                AttendanceDate = date
            };


            var result = await _dapper.GetAllAsyncs<dynamic>(query, parameters, commandType: CommandType.StoredProcedure);
            return result;
        }




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
                Type = type,
                startDate = startDate,
                endDate = endDate
            };

            var result = await _dapper.GetAllAsync<CombinedUsageDto>(query, parameters);

            return result;
        }
        public async Task<List<AppUsage>> GetAppUsages(int OrganizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate)
        {
            string appUsageQuery = @"
           SELECT 
               A.UserId, 
               A.ApplicationName, 
               A.Details, 
 SUM(
        -- Convert TotalUsage into total seconds manually
        CAST(SUBSTRING(A.TotalUsage, 1, CHARINDEX(':', A.TotalUsage) - 1) AS INT) * 3600 +  -- Hours to seconds
        CAST(SUBSTRING(A.TotalUsage, CHARINDEX(':', A.TotalUsage) + 1, CHARINDEX(':', A.TotalUsage, CHARINDEX(':', A.TotalUsage) + 1) - CHARINDEX(':', A.TotalUsage) - 1) AS INT) * 60 +  -- Minutes to seconds
        CAST(SUBSTRING(A.TotalUsage, CHARINDEX(':', A.TotalUsage, CHARINDEX(':', A.TotalUsage) + 1) + 1, LEN(A.TotalUsage)) AS INT)  -- Seconds
    ) AS TotalSeconds, 
               A.UsageDate
           FROM  
               ApplicationUsage A
           INNER JOIN 
               Users U ON A.UserId = U.Id
           INNER JOIN 
                   Team T ON T.Id = U.TeamId
           INNER JOIN 
               Organization O ON U.OrganizationId = O.Id
           WHERE  
                  O.Id = @OrganizationId 
                  AND A.UsageDate BETWEEN @FromDate AND @ToDate
                  AND (@TeamId IS NULL OR T.Id = @TeamId)
                  AND (@UserId IS NULL OR A.UserId = @UserId)
           GROUP BY 
               A.UserId, 
               A.ApplicationName, 
               A.Details, 
               A.UsageDate;
        ";

            var urlUsageQuery = @"
    SELECT 
        U.UserId,
        U.Url AS ApplicationName,
        NULL AS Details,
 SUM(
        CAST(SUBSTRING( U.TotalUsage, 1, CHARINDEX(':', U.TotalUsage) - 1) AS INT) * 3600 +  -- Hours to seconds
        CAST(SUBSTRING( U.TotalUsage, CHARINDEX(':',  U.TotalUsage) + 1, CHARINDEX(':',  U.TotalUsage, CHARINDEX(':', U.TotalUsage) + 1) - CHARINDEX(':',  U.TotalUsage) - 1) AS INT) * 60 +  -- Minutes to seconds
        CAST(SUBSTRING( U.TotalUsage, CHARINDEX(':', U.TotalUsage, CHARINDEX(':',  U.TotalUsage) + 1) + 1, LEN( U.TotalUsage)) AS INT)  -- Seconds
    ) AS TotalSeconds, 
        U.UsageDate
    FROM 
        UrlUsage U
    INNER JOIN 
        Users Us ON U.UserId = Us.Id
    INNER JOIN 
        Team T ON T.Id = Us.TeamId
    INNER JOIN 
        Organization O ON Us.OrganizationId = O.Id
    WHERE 
        O.Id = @OrganizationId 
        AND U.UsageDate BETWEEN @FromDate AND @ToDate
        AND (@TeamId IS NULL OR T.Id = @TeamId)
        AND (@UserId IS NULL OR U.UserId = @UserId)
    GROUP BY 
        U.UserId,
        U.Url,
        U.UsageDate;
";
            ;
            var parameters = new
            {
                OrganizationId = OrganizationId,
                TeamId = teamId,
                UserId = userId,
                FromDate = fromDate,
                ToDate = toDate
            };

            var appUsages = await _dapper.GetAllAsync<AppUsage>(appUsageQuery, parameters);
            var urlUsages = await _dapper.GetAllAsync<AppUsage>(urlUsageQuery, parameters);

            var allUsages = appUsages.Concat(urlUsages);

            var groupedUsages = allUsages
     .GroupBy(u => u.ApplicationName.ToLower()) // Grouping by URL in lowercase for case-insensitivity
     .Select(g => new AppUsage
     {
         UserId = g.First().UserId, // You can choose how to handle UserId if there are multiple
         ApplicationName = g.Key, // Use the grouped ApplicationName
         Details = g.First().Details, // Keep the first Details (adjust as needed)
         TotalSeconds = g.Sum(u => u.TotalSeconds), // Sum the TotalSeconds for the same URL
         UsageDate = g.Min(u => u.UsageDate) // Optional: Use earliest date as the representative date
     })
     .ToList();

            // If you want to return a List<AppUsage>, no need to create a dictionary
            return groupedUsages;


        }

        public async Task<List<dynamic>> DynamicReport([FromQuery] DynamicReportRequest request)
        {
            var query = "DynamicReport";

            var parameters = new
            {
                request.OrganizationId,
                request.TeamId,
                request.UserId,
                request.StartDate,
                request.EndDate,
                FirstName = request.FirstName.HasValue && request.FirstName.Value ? 1 : 0,
                LastName = request.LastName.HasValue && request.LastName.Value ? 1 : 0,
                EmployeeId = request.EmployeeId.HasValue && request.EmployeeId.Value ? 1 : 0,
                Email = request.Email.HasValue && request.Email.Value ? 1 : 0,
                TeamName = request.TeamName.HasValue && request.TeamName.Value ? 1 : 0,
                Manager = request.Manager.HasValue && request.Manager.Value ? 1 : 0,
                TotalWorkingtime = request.TotalWorkingtime.HasValue && request.TotalWorkingtime.Value ? 1 : 0,
                TotalOnlinetime = request.TotalOnlinetime.HasValue && request.TotalOnlinetime.Value ? 1 : 0,
                TotalBreaktime = request.TotalBreaktime.HasValue && request.TotalBreaktime.Value ? 1 : 0,
                AverageBreaktime = request.AverageBreaktime.HasValue && request.AverageBreaktime.Value ? 1 : 0,
                TotalActivetime = request.TotalActivetime.HasValue && request.TotalActivetime.Value ? 1 : 0,
                AverageActivetime = request.AverageActivetime.HasValue && request.AverageActivetime.Value ? 1 : 0,
                ActivitePercent = request.ActivitePercent.HasValue && request.ActivitePercent.Value ? 1 : 0,
                TotalIdletime = request.TotalIdletime.HasValue && request.TotalIdletime.Value ? 1 : 0,
                AverageIdletime = request.AverageIdletime.HasValue && request.AverageIdletime.Value ? 1 : 0
            };

            var result = new List<dynamic>();
            var result1 = await _dapper.GetAllAsync<DynamicReportRequest>(query, parameters);

            var productivityData = await _dapper.GetAllAsyncs<dynamic>("uspGetDynamicProductivity", new
            {
                OrganizationId = request.OrganizationId,
                TeamId = request.TeamId,
                UserId = request.UserId,
                FromDate = request.StartDate,
                ToDate = request.EndDate
            }, commandType: CommandType.StoredProcedure);

            var productivityLookup = productivityData
                .Where(x => x.UserID != null)
                .GroupBy(x => (int)x.UserID)
                .ToDictionary(g => g.Key, g => g.First());

            var dateDifferenceInDays = (request.EndDate - request.StartDate).TotalDays;
            if (dateDifferenceInDays <= 0) dateDifferenceInDays = 1;

            foreach (var record in result1)
            {
                var dynamicItem = new ExpandoObject() as dynamic;
                dynamicItem.UserId = record.UserId;
                dynamicItem.TeamId = record.TeamId;

                if (request.FirstName == true)
                    dynamicItem.FirstName = record.FirstNameOutput;

                if (request.LastName == true)
                    dynamicItem.LastName = record.LastNameOutput;

                if (request.EmployeeId == true)
                    dynamicItem.EmployeeId = record.EmployeeIdOutput;

                if (request.Email == true)
                    dynamicItem.Email = record.EmailOutput;

                if (request.TeamName == true)
                    dynamicItem.TeamName = record.TeamNameOutput;

                if (request.TotalWorkingtime == true)
                    dynamicItem.TotalWorkingtime = record.TotalWorkingtimeOutput;

                if (request.TotalOnlinetime == true)
                    dynamicItem.TotalOnlinetime = record.TotalOnlinetimeOutput;

                if (request.TotalBreaktime == true)
                    dynamicItem.TotalBreaktime = record.TotalBreaktimeOutput;

                if (request.AverageBreaktime == true)
                    dynamicItem.AverageBreaktime = record.AverageBreaktimeOutput;

                if (request.TotalActivetime == true)
                    dynamicItem.TotalActivetime = record.TotalActivetimeOutput;

                if (request.AverageActivetime == true)
                    dynamicItem.AverageActivetime = record.AverageActivetimeOutput;

                if (request.ActivitePercent == true)
                    dynamicItem.ActivitePercent = record.ActivitePercentOutput;

                if (request.TotalIdletime == true)
                    dynamicItem.TotalIdletime = record.TotalIdletimeOutput;

                if (request.AverageIdletime == true)
                    dynamicItem.AverageIdletime = record.AverageIdletimeOutput;

                if (productivityLookup.TryGetValue((int)record.UserId, out var productivity))
                {
                    int totalProductiveDuration = productivity.TotalProductiveDuration;
                    int totalUnproductiveDuration = productivity.TotalUnproductiveDuration;
                    int totalNeutralDuration = productivity.TotalNeutralDuration;

                    double? activeDurationInSeconds = productivity.TotalWorkingTime ?? 0.0;
                    double percentageProductiveDuration = activeDurationInSeconds > 0
                        ? ((double)totalProductiveDuration / activeDurationInSeconds.Value) * 100
                        : 0.0;

                    double averageProductiveDuration = totalProductiveDuration / dateDifferenceInDays;
                    double averageUnproductiveDuration = totalUnproductiveDuration / dateDifferenceInDays;
                    double averageNeutralDuration = totalNeutralDuration / dateDifferenceInDays;

                    if (request.TotalProductivetime == true)
                        dynamicItem.Total_Productivetime = FormatDuration(totalProductiveDuration);

                    if (request.ProductivityPercent == true)
                        dynamicItem.Productivity_Percent = percentageProductiveDuration;

                    if (request.AverageProductivetime == true)
                        dynamicItem.Average_Productivetime = FormatDuration((long)averageProductiveDuration);

                    if (request.Totalunproductivetime == true)
                        dynamicItem.Total_unproductivetime = FormatDuration(totalUnproductiveDuration);

                    if (request.Averageunproductivetime == true)
                        dynamicItem.Average_unproductivetime = FormatDuration((long)averageUnproductiveDuration);

                    if (request.Totalneutraltime == true)
                        dynamicItem.Total_neutraltime = FormatDuration(totalNeutralDuration);

                    if (request.Averageneutraltime == true)
                        dynamicItem.Average_neutraltime = FormatDuration((long)averageNeutralDuration);
                }
                else
                {
                    if (request.TotalProductivetime == true)
                        dynamicItem.Total_Productivetime = FormatDuration(0);

                    if (request.ProductivityPercent == true)
                        dynamicItem.Productivity_Percent = 0;

                    if (request.AverageProductivetime == true)
                        dynamicItem.Average_Productivetime = FormatDuration(0);

                    if (request.Totalunproductivetime == true)
                        dynamicItem.Total_unproductivetime = FormatDuration(0);

                    if (request.Averageunproductivetime == true)
                        dynamicItem.Average_unproductivetime = FormatDuration(0);

                    if (request.Totalneutraltime == true)
                        dynamicItem.Total_neutraltime = FormatDuration(0);

                    if (request.Averageneutraltime == true)
                        dynamicItem.Average_neutraltime = FormatDuration(0);
                }

                result.Add(new { records = dynamicItem });
            }

            return result;
        }


        public async Task<List<dynamic>> DynamicDetailReport([FromQuery] DynamicReportRequest request)
        {
            var query = "DynamicDetailReport";

            var parameters = new
            {
                request.OrganizationId,
                request.TeamId,
                request.UserId,
                request.StartDate,
                request.EndDate,
                FirstName = request.FirstName.HasValue && request.FirstName.Value ? 1 : 0,
                LastName = request.LastName.HasValue && request.LastName.Value ? 1 : 0,
                EmployeeId = request.EmployeeId.HasValue && request.EmployeeId.Value ? 1 : 0,
                Email = request.Email.HasValue && request.Email.Value ? 1 : 0,
                TeamName = request.TeamName.HasValue && request.TeamName.Value ? 1 : 0,
                Manager = request.Manager.HasValue && request.Manager.Value ? 1 : 0,
                TotalWorkingtime = request.TotalWorkingtime.HasValue && request.TotalWorkingtime.Value ? 1 : 0,
                TotalOnlinetime = request.TotalOnlinetime.HasValue && request.TotalOnlinetime.Value ? 1 : 0,
                TotalBreaktime = request.TotalBreaktime.HasValue && request.TotalBreaktime.Value ? 1 : 0,
                TotalActivetime = request.TotalActivetime.HasValue && request.TotalActivetime.Value ? 1 : 0,
                ActivitePercent = request.ActivitePercent.HasValue && request.ActivitePercent.Value ? 1 : 0,
                TotalIdletime = request.TotalIdletime.HasValue && request.TotalIdletime.Value ? 1 : 0,
                PunchIntime = request.PunchIntime.HasValue && request.PunchIntime.Value ? 1 : 0,
                PunchOuttime = request.PunchOuttime.HasValue && request.PunchOuttime.Value ? 1 : 0
            };

            var result = new List<dynamic>();
            var result1 = await _dapper.GetAllAsync<DynamicReportRequest>(query, parameters);

            // Get all productivity data in one call
            var productivityData = await _dapper.GetAllAsyncs<dynamic>("uspGetDynamicDetailProductivity", new
            {
                OrganizationId = request.OrganizationId,
                TeamId = request.TeamId,
                UserId = request.UserId,
                FromDate = request.StartDate,
                ToDate = request.EndDate
            }, commandType: CommandType.StoredProcedure);

            // Prepare a dictionary for fast lookup
            var productivityLookup = productivityData
            .Where(x => x.UserID != null && x.AttendanceDate != null) // <-- Filter nulls
            .GroupBy(x => new { UserID = (int)x.UserID, Date = (DateTime)x.AttendanceDate })
            .ToDictionary(
                g => g.Key,
                g => g.First()
            );

            // Pre-calculate date difference
            var dateDifferenceInDays = (request.EndDate - request.StartDate).TotalDays;
            if (dateDifferenceInDays <= 0) dateDifferenceInDays = 1;

            foreach (var record in result1)
            {
                var dynamicItem = new ExpandoObject() as dynamic;
                dynamicItem.Date = record.Date;
                dynamicItem.UserId = record.UserId;
                dynamicItem.TeamId = record.TeamId;

                if (request.FirstName == true)
                    dynamicItem.FirstName = record.FirstNameOutput;

                if (request.LastName == true)
                    dynamicItem.LastName = record.LastNameOutput;

                if (request.EmployeeId == true)
                    dynamicItem.EmployeeId = record.EmployeeIdOutput;

                if (request.Email == true)
                    dynamicItem.Email = record.EmailOutput;

                if (request.TeamName == true)
                    dynamicItem.TeamName = record.TeamNameOutput;

                if (request.TotalWorkingtime == true)
                    dynamicItem.TotalWorkingtime = record.TotalWorkingtimeOutput;

                if (request.TotalOnlinetime == true)
                    dynamicItem.TotalOnlinetime = record.TotalOnlinetimeOutput;

                if (request.TotalBreaktime == true)
                    dynamicItem.TotalBreaktime = record.TotalBreaktimeOutput;

                if (request.TotalActivetime == true)
                    dynamicItem.TotalActivetime = record.TotalActivetimeOutput;

                if (request.ActivitePercent == true)
                    dynamicItem.ActivitePercent = record.ActivitePercentOutput;

                if (request.TotalIdletime == true)
                    dynamicItem.TotalIdletime = record.TotalIdletimeOutput;

                if (request.PunchIntime == true)
                    dynamicItem.PunchIntime = record.PunchIntimeOutput;

                if (request.PunchOuttime == true)
                    dynamicItem.PunchOuttime = record.PunchOuttimeOutput;

                var key = new { UserID = (int)record.UserId, Date = record.Date };
                if (productivityLookup.TryGetValue(key, out var productivity))
                {
                    int totalProductiveDuration = productivity.TotalProductiveDuration;
                    int totalUnproductiveDuration = productivity.TotalUnproductiveDuration;
                    int totalNeutralDuration = productivity.TotalNeutralDuration;

                    double? activeDurationInSeconds = productivity.TotalWorkingTime ?? 0.0;
                    double percentageProductiveDuration = activeDurationInSeconds > 0
                        ? ((double)totalProductiveDuration / activeDurationInSeconds.Value) * 100
                        : 0.0;

                    double averageProductiveDuration = totalProductiveDuration / dateDifferenceInDays;
                    double averageUnproductiveDuration = totalUnproductiveDuration / dateDifferenceInDays;
                    double averageNeutralDuration = totalNeutralDuration / dateDifferenceInDays;

                    if (request.TotalProductivetime == true)
                        dynamicItem.Total_Productivetime = FormatDuration(totalProductiveDuration);

                    if (request.ProductivityPercent == true)
                        dynamicItem.Productivity_Percent = percentageProductiveDuration;

                    if (request.AverageProductivetime == true)
                        dynamicItem.Average_Productivetime = FormatDuration((long)averageProductiveDuration);

                    if (request.Totalunproductivetime == true)
                        dynamicItem.Total_unproductivetime = FormatDuration(totalUnproductiveDuration);

                    if (request.Averageunproductivetime == true)
                        dynamicItem.Average_unproductivetime = FormatDuration((long)averageUnproductiveDuration);

                    if (request.Totalneutraltime == true)
                        dynamicItem.Total_neutraltime = FormatDuration(totalNeutralDuration);

                    if (request.Averageneutraltime == true)
                        dynamicItem.Average_neutraltime = FormatDuration((long)averageNeutralDuration);
                }
                else
                {
                    if (request.TotalProductivetime == true)
                        dynamicItem.Total_Productivetime = FormatDuration(0);

                    if (request.ProductivityPercent == true)
                        dynamicItem.Productivity_Percent = 0;

                    if (request.AverageProductivetime == true)
                        dynamicItem.Average_Productivetime = FormatDuration(0);

                    if (request.Totalunproductivetime == true)
                        dynamicItem.Total_unproductivetime = FormatDuration(0);

                    if (request.Averageunproductivetime == true)
                        dynamicItem.Average_unproductivetime = FormatDuration(0);

                    if (request.Totalneutraltime == true)
                        dynamicItem.Total_neutraltime = FormatDuration(0);

                    if (request.Averageneutraltime == true)
                        dynamicItem.Average_neutraltime = FormatDuration(0);
                }
                result.Add(new { records = dynamicItem });
            }
            return result;
        }
    }
}
