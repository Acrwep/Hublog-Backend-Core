using Hublog.Repository.Common;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Dynamic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            var query = "GetAttendanceReport1234";

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
            var result2 = new List<dynamic>();
            var result23 = new List<dynamic>();
            var result1 = await _dapper.GetAllAsync<DynamicReportRequest>(query, parameters);
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

                result.Add(dynamicItem);
            }

            var teamQuery = @"
                   SELECT T.Id,T.Name
                    FROM Team T
                    INNER JOIN Organization O ON T.OrganizationId = O.Id
                    WHERE O.Id = @OrganizationId
                    AND (@TeamId IS NULL OR T.Id = @TeamId) ";

            var teams = await _dapper.GetAllAsync<(int TeamId, string TeamName)>(teamQuery, new { OrganizationId = request.OrganizationId, TeamId = request.TeamId });

            List<dynamic> groupedUsages = new List<dynamic>();
            if (request.UserId.HasValue)
            {
                foreach (var team in teams)
                {
                    var TeamName = team.TeamName;
                    var teamId = team.TeamId;

                    var urlUsageQuery = "GetAppUsagesSS";
                    var parameters1 = new
                    {
                        OrganizationId = request.OrganizationId,
                        TeamId = teamId,
                        UserId = request.UserId,
                        FromDate = request.StartDate,
                        ToDate = request.EndDate
                    };

                    IEnumerable<dynamic> results = await _dapper.GetAllAsync<dynamic>(urlUsageQuery, parameters1);
                    var getUsers = @"
                SELECT id AS UserId, CONCAT(First_Name, ' ', Last_Name) AS FullName 
                FROM Users 
                WHERE TeamId = @TeamId
                AND (@UserId IS NULL OR Id = @UserId)";

                    var getUsers1 = await _dapper.GetAllAsync<dynamic>(getUsers, parameters1);

                    var combinedResults = results.Concat(getUsers1)
                        .GroupBy(item => item.UserId)
                        .Select(group =>
                        {
                            var usageEntry = results.FirstOrDefault(u => u.UserId == group.Key);
                            return usageEntry ?? group.First();
                        })
                        .ToList();

                    foreach (var us in combinedResults)
                    {
                        var userIdd = us.UserId;
                        var FullName = us.FullName;
                        int totalProductiveDuration = 0, totalUnproductiveDuration = 0, totalNeutralDuration = 0;
                        var organizationId = request.OrganizationId;
                        var userId = us.UserId;
                        var fromDate = request.StartDate;
                        var toDate = request.EndDate;
                        // var usages = await GetAppUsages(request.OrganizationId, teamId, userIdd, request.StartDate, request.EndDate);
                        var usages = await GetAppUsages(organizationId, teamId, userId, fromDate, toDate);

                        foreach (var usage in usages)
                        {
                            var totalSeconds = usage.TotalSeconds;
                            usage.ApplicationName = usage.ApplicationName.ToLower();

                            if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge" && usage.ApplicationName != "firefox" && usage.ApplicationName != "opera")
                            {

                                    usage.TotalSeconds = totalSeconds;
                                    usage.TotalUsage = TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");

                                var parameters2 = new { ApplicationName = usage.ApplicationName.ToLower() };
                                var app = await _dapper.QueryFirstOrDefaultAsync<string>("GetApplicationCategoryAndProductivity", parameters2, commandType: CommandType.StoredProcedure);

                                if (app != null)
                                {
                                    switch (app)
                                    {
                                        case "Productive":
                                            totalProductiveDuration += usage.TotalSeconds;
                                            break;
                                        case "Unproductive":
                                            totalUnproductiveDuration += usage.TotalSeconds;
                                            break;
                                        case "Neutral":
                                            totalNeutralDuration += usage.TotalSeconds;
                                            break;
                                    }
                                }
                            }
                        }
                        var onlineDurationInSeconds = us.OnlineDurationInHours ?? 0.0;
                        double? activeDurationInSeconds = us.ActiveTimeInSeconds ?? 0.0;
                        var totalDurationInSeconds = totalProductiveDuration + totalUnproductiveDuration + totalNeutralDuration;

                        var dateDifferenceInDays = (request.EndDate - request.StartDate).TotalDays;
                        if (dateDifferenceInDays <= 0)
                        {
                            dateDifferenceInDays = 1;
                        }

                        double percentageProductiveDuration = activeDurationInSeconds > 0 ? ((double)totalProductiveDuration / activeDurationInSeconds.Value) * 100 : 0.0;
                        double averageProductiveDuration = dateDifferenceInDays > 0 ? (double)totalProductiveDuration / dateDifferenceInDays : 0.0;
                        double averageUnproductiveDuration = dateDifferenceInDays > 0 ? (double)totalUnproductiveDuration / dateDifferenceInDays : 0.0;
                        double averageNeutralDuration = dateDifferenceInDays > 0 ? (double)totalNeutralDuration / dateDifferenceInDays : 0.0;

                        string FormatDuration(double totalSeconds)
                        {
                            var hours = (long)(totalSeconds / 3600);
                            var minutes = (long)((totalSeconds % 3600) / 60);
                            var seconds = (long)(totalSeconds % 60);
                            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
                        }

                        var dynamicItem = new ExpandoObject() as dynamic;
                        dynamicItem.UserId = userIdd;
                        //dynamicItem.Full_Name = FullName;
                        //dynamicItem.Team_Name = TeamName;

                        //dynamicItem.ActiveDuration = FormatDuration(activeDurationInSeconds ?? 0.0);
                        //dynamicItem.OnlineDuration = FormatDuration(onlineDurationInSeconds);

                        dynamicItem.Total_Productivetime = FormatDuration(totalProductiveDuration);
                        dynamicItem.TotalUnproductiveDuration = FormatDuration(totalUnproductiveDuration);
                        dynamicItem.TotalNeutralDuration = FormatDuration(totalNeutralDuration);
                        dynamicItem.TotalDuration = FormatDuration(totalDurationInSeconds);
                        dynamicItem.PercentageProductiveDuration = percentageProductiveDuration;
                        dynamicItem.AverageProductiveDuration = FormatDuration(averageProductiveDuration);
                        dynamicItem.AverageUnproductiveDuration = FormatDuration(averageUnproductiveDuration);
                        dynamicItem.AverageNeutralDuration = FormatDuration(averageNeutralDuration);

                        result23.Add(dynamicItem);
                    }
                }
                foreach (var record in result23)
                {
                    var dynamicItem = new ExpandoObject() as dynamic;
                    dynamicItem.UserId = record.UserId;

                    // Check and add formatted durations and percentage
                    if (request.TotalProductivetime == true)
                        dynamicItem.Total_Productivetime = record.Total_Productivetime;

                    if (request.ProductivityPercent == true)
                        dynamicItem.Productivity_Percent = record.PercentageProductiveDuration;

                    if (request.AverageProductivetime == true)
                        dynamicItem.Average_Productivetime = record.AverageProductiveDuration;

                    if (request.Totalunproductivetime == true)
                        dynamicItem.Total_unproductivetime = record.TotalUnproductiveDuration;

                    if (request.Averageunproductivetime == true)
                        dynamicItem.Average_unproductivetime = record.AverageUnproductiveDuration;

                    if (request.Totalneutraltime == true)
                        dynamicItem.Total_neutraltime = record.TotalNeutralDuration;

                    if (request.Averageneutraltime == true)
                        dynamicItem.Average_neutraltime =record.AverageNeutralDuration;

                    result2.Add(dynamicItem);
                }

                var cc = result.Concat(result2);
                groupedUsages = cc
                    .GroupBy(u => u.UserId) // Group by UserId
                    .Select(g => new
                    {
                        Records = g.Aggregate(new ExpandoObject() as IDictionary<string, object>, (acc, item) =>
                        {
                            foreach (var prop in (IDictionary<string, object>)item)
                            {
                                // Add the property to the accumulator (ExpandoObject)
                                if (!acc.ContainsKey(prop.Key))
                                {
                                    acc.Add(prop.Key, prop.Value);
                                }
                            }
                            return acc;
                        })
                    })
                    .Select(g => new
                    {
                        Records = g.Records // The merged records for each UserId
                    })
                    .ToList<dynamic>();
            }
            else
            {
                foreach (var team in teams)
                {
                    var TeamName = team.TeamName;
                    var teamId = team.TeamId;

                    var urlUsageQuery = "GetAppUsagesSS";
                    var parameters1 = new
                    {
                        OrganizationId = request.OrganizationId,
                        TeamId = teamId,
                        UserId = request.UserId,
                        FromDate = request.StartDate,
                        ToDate = request.EndDate
                    };

                    IEnumerable<dynamic> results = await _dapper.GetAllAsync<dynamic>(urlUsageQuery, parameters1);
                    var getUsers = @"
                SELECT id AS UserId, CONCAT(First_Name, ' ', Last_Name) AS FullName 
                FROM Users 
                WHERE TeamId = @TeamId
                AND (@UserId IS NULL OR Id = @UserId)";

                    var getUsers1 = await _dapper.GetAllAsync<dynamic>(getUsers, parameters1);

                    var combinedResults = results.Concat(getUsers1)
                        .GroupBy(item => item.UserId)
                        .Select(group =>
                        {
                            var usageEntry = results.FirstOrDefault(u => u.UserId == group.Key);
                            return usageEntry ?? group.First();
                        })
                        .ToList();

                    foreach (var us in combinedResults)
                    {
                        var userIdd = us.UserId;
                        var FullName = us.FullName;
                        int totalProductiveDuration = 0, totalUnproductiveDuration = 0, totalNeutralDuration = 0;

                        var usages = await GetAppUsages(request.OrganizationId, teamId, userIdd, request.StartDate, request.EndDate);

                        foreach (var usage in usages)
                        {
                            var totalSeconds = usage.TotalSeconds;
                            usage.ApplicationName = usage.ApplicationName.ToLower();

                            if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge" && usage.ApplicationName != "firefox" && usage.ApplicationName != "opera")
                            {

                                usage.TotalSeconds = totalSeconds;
                                usage.TotalUsage = TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");



                                var parameters2 = new { ApplicationName = usage.ApplicationName.ToLower() };
                                var app = await _dapper.QueryFirstOrDefaultAsync<string>("GetApplicationCategoryAndProductivity", parameters2, commandType: CommandType.StoredProcedure);

                                if (app != null)
                                {
                                    switch (app)
                                    {
                                        case "Productive":
                                            totalProductiveDuration += usage.TotalSeconds;
                                            break;
                                        case "Unproductive":
                                            totalUnproductiveDuration += usage.TotalSeconds;
                                            break;
                                        case "Neutral":
                                            totalNeutralDuration += usage.TotalSeconds;
                                            break;
                                    }
                                }
                            }
                        }

                        var AttendanceCount = us.AttendanceCount;
                        var onlineDurationInSeconds = us.OnlineDurationInHours ?? 0.0;
                        double? activeDurationInSeconds = us.ActiveTimeInSeconds ?? 0.0;
                        var breakDurationInSeconds = us.TotalBreakDurationInSeconds ?? 0.0;
                        var totalDurationInSeconds = totalProductiveDuration + totalUnproductiveDuration + totalNeutralDuration;

                        var dateDifferenceInDays = (request.EndDate - request.StartDate).TotalDays;
                        if (dateDifferenceInDays <= 0)
                        {
                            dateDifferenceInDays = 1;
                        }

                        var percentageProductiveDuration = activeDurationInSeconds > 0 ? ((double)totalProductiveDuration / activeDurationInSeconds.Value) * 100 : 0.0;
                        double averageProductiveDuration = dateDifferenceInDays > 0 ? (double)totalProductiveDuration / dateDifferenceInDays : 0.0;
                        double averageUnproductiveDuration = dateDifferenceInDays > 0 ? (double)totalUnproductiveDuration / dateDifferenceInDays : 0.0;
                        double averageNeutralDuration = dateDifferenceInDays > 0 ? (double)totalNeutralDuration / dateDifferenceInDays : 0.0;

                        string FormatDuration(double totalSeconds)
                        {
                            var hours = (long)(totalSeconds / 3600);
                            var minutes = (long)((totalSeconds % 3600) / 60);
                            var seconds = (long)(totalSeconds % 60);
                            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
                        }

                        var dynamicItem = new ExpandoObject() as dynamic;
                        dynamicItem.UserId = userIdd;
                        //dynamicItem.Full_Name = FullName;
                        //dynamicItem.Team_Name = TeamName;
                        //dynamicItem.AttendanceCount = AttendanceCount;

                        //dynamicItem.ActiveDuration = FormatDuration(activeDurationInSeconds ?? 0.0);
                        //dynamicItem.BreakDuration = FormatDuration(breakDurationInSeconds);
                        //dynamicItem.OnlineDuration = FormatDuration(onlineDurationInSeconds);
                        dynamicItem.Total_Productivetime = FormatDuration(totalProductiveDuration);
                        dynamicItem.TotalUnproductiveDuration = FormatDuration(totalUnproductiveDuration);
                        dynamicItem.TotalNeutralDuration = FormatDuration(totalNeutralDuration);
                        dynamicItem.TotalDuration = FormatDuration(totalDurationInSeconds);
                        dynamicItem.PercentageProductiveDuration = percentageProductiveDuration;
                        dynamicItem.AverageProductiveDuration = FormatDuration(averageProductiveDuration);
                        dynamicItem.AverageUnproductiveDuration = FormatDuration(averageUnproductiveDuration);
                        dynamicItem.AverageNeutralDuration = FormatDuration(averageNeutralDuration);

                        result23.Add(dynamicItem);
                    }

                }
                foreach (var record in result23)
                {
                    var dynamicItem = new ExpandoObject() as dynamic;
                    dynamicItem.UserId = record.UserId;

                    // Check and add formatted durations and percentage
                    if (request.TotalProductivetime == true)
                        dynamicItem.Total_Productivetime = record.Total_Productivetime;

                    if (request.ProductivityPercent == true)
                        dynamicItem.Productivity_Percent = record.PercentageProductiveDuration;

                    if (request.AverageProductivetime == true)
                        dynamicItem.Average_Productivetime = record.AverageProductiveDuration;

                    if (request.Totalunproductivetime == true)
                        dynamicItem.Total_unproductivetime = record.TotalUnproductiveDuration;

                    if (request.Averageunproductivetime == true)
                        dynamicItem.Average_unproductivetime = record.AverageUnproductiveDuration;

                    if (request.Totalneutraltime == true)
                        dynamicItem.Total_neutraltime = record.TotalNeutralDuration;

                    if (request.Averageneutraltime == true)
                        dynamicItem.Average_neutraltime = record.AverageNeutralDuration;

                    result2.Add(dynamicItem);
                    result.Add(dynamicItem);
                }
                var cc = result.Concat(result2);
                groupedUsages = cc
                    .GroupBy(u => u.UserId) // Group by UserId
                    .Select(g => new
                    {
                        Records = g.Aggregate(new ExpandoObject() as IDictionary<string, object>, (acc, item) =>
                        {
                            foreach (var prop in (IDictionary<string, object>)item)
                            {
                                // Add the property to the accumulator (ExpandoObject)
                                if (!acc.ContainsKey(prop.Key))
                                {
                                    acc.Add(prop.Key, prop.Value);
                                }
                            }
                            return acc;
                        })
                    })
                    .ToList<dynamic>();
            }

                return groupedUsages;
        }
        public async Task<List<dynamic>> DynamicDetailReport([FromQuery] DynamicReportRequest request)
        {
            var query = "GetAttendanceReport012";

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
            var result2 = new List<dynamic>();
            var result23 = new List<dynamic>();
            var result1 = await _dapper.GetAllAsync<DynamicReportRequest>(query, parameters);
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

                result.Add(dynamicItem);
            }

            var teamQuery = @"
                   SELECT T.Id,T.Name
                    FROM Team T
                    INNER JOIN Organization O ON T.OrganizationId = O.Id
                    WHERE O.Id = @OrganizationId
                    AND (@TeamId IS NULL OR T.Id = @TeamId) ";

            var teams = await _dapper.GetAllAsync<(int TeamId, string TeamName)>(teamQuery, new { OrganizationId = request.OrganizationId, TeamId = request.TeamId });

            List<dynamic> groupedUsages = new List<dynamic>();
            if (request.UserId.HasValue)
            {
                foreach (var team in teams)
                {
                    var TeamName = team.TeamName;
                    var teamId = team.TeamId;

                    var urlUsageQuery = "Datewise_Activity1";
                    var parameters1 = new
                    {
                        OrganizationId = request.OrganizationId,
                        TeamId = teamId,
                        UserId = request.UserId,
                        FromDate = request.StartDate,
                        ToDate = request.EndDate
                    };

                    IEnumerable<dynamic> results = await _dapper.GetAllAsync<dynamic>(urlUsageQuery, parameters1);
                //    var getUsers = @"
                //SELECT id AS UserId, CONCAT(First_Name, ' ', Last_Name) AS FullName 
                //FROM Users 
                //WHERE TeamId = @TeamId
                //AND (@UserId IS NULL OR Id = @UserId)";

                //    var getUsers1 = await _dapper.GetAllAsync<dynamic>(getUsers, parameters1);

                    ////var combinedResults = results.Concat(getUsers1)
                    //    .GroupBy(item => item.UserId)
                    //    .Select(group =>
                    //    {
                    //        var usageEntry = results.FirstOrDefault(u => u.UserId == group.Key);
                    //        return usageEntry ?? group.First();
                    //    })
                    //    .ToList();

                    foreach (var us in results)
                    {
                        var userIdd = us.UserId;
                        var FullName = us.FullName;
                        int totalProductiveDuration = 0, totalUnproductiveDuration = 0, totalNeutralDuration = 0;
                        var organizationId = request.OrganizationId;
                        var userId = us.UserId;
                        var fromDate = us.Date;
                        var toDate = us.Date;
                        // var usages = await GetAppUsages(request.OrganizationId, teamId, userIdd, request.StartDate, request.EndDate);
                        var usages = await GetAppUsages(organizationId, teamId, userId, fromDate, toDate);

                        foreach (var usage in usages)
                        {
                            var totalSeconds = usage.TotalSeconds;
                            usage.ApplicationName = usage.ApplicationName.ToLower();

                            if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge" && usage.ApplicationName != "firefox" && usage.ApplicationName != "opera")
                            {

                                usage.TotalSeconds = totalSeconds;
                                usage.TotalUsage = TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");

                                var parameters2 = new { ApplicationName = usage.ApplicationName.ToLower() };
                                var app = await _dapper.QueryFirstOrDefaultAsync<string>("GetApplicationCategoryAndProductivity", parameters2, commandType: CommandType.StoredProcedure);

                                if (app != null)
                                {
                                    switch (app)
                                    {
                                        case "Productive":
                                            totalProductiveDuration += usage.TotalSeconds;
                                            break;
                                        case "Unproductive":
                                            totalUnproductiveDuration += usage.TotalSeconds;
                                            break;
                                        case "Neutral":
                                            totalNeutralDuration += usage.TotalSeconds;
                                            break;
                                    }
                                }
                            }
                        }
                        var onlineDurationInSeconds = us.OnlineDurationInHours ?? 0.0;
                        double? activeDurationInSeconds = us.ActiveTimeInSeconds ?? 0.0;
                        var totalDurationInSeconds = totalProductiveDuration + totalUnproductiveDuration + totalNeutralDuration;

                        var dateDifferenceInDays = (request.EndDate - request.StartDate).TotalDays;
                        if (dateDifferenceInDays <= 0)
                        {
                            dateDifferenceInDays = 1;
                        }

                        double percentageProductiveDuration = activeDurationInSeconds > 0 ? ((double)totalProductiveDuration / activeDurationInSeconds.Value) * 100 : 0.0;
                        double averageProductiveDuration = dateDifferenceInDays > 0 ? (double)totalProductiveDuration / dateDifferenceInDays : 0.0;
                        double averageUnproductiveDuration = dateDifferenceInDays > 0 ? (double)totalUnproductiveDuration / dateDifferenceInDays : 0.0;
                        double averageNeutralDuration = dateDifferenceInDays > 0 ? (double)totalNeutralDuration / dateDifferenceInDays : 0.0;

                        string FormatDuration(double totalSeconds)
                        {
                            var hours = (long)(totalSeconds / 3600);
                            var minutes = (long)((totalSeconds % 3600) / 60);
                            var seconds = (long)(totalSeconds % 60);
                            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
                        }

                        var dynamicItem = new ExpandoObject() as dynamic;
                        dynamicItem.UserId = userIdd;
                        dynamicItem.Date = fromDate;
                        //dynamicItem.Full_Name = FullName;
                        //dynamicItem.Team_Name = TeamName;

                        //dynamicItem.ActiveDuration = FormatDuration(activeDurationInSeconds ?? 0.0);
                        //dynamicItem.OnlineDuration = FormatDuration(onlineDurationInSeconds);

                        dynamicItem.Total_Productivetime = FormatDuration(totalProductiveDuration);
                        dynamicItem.TotalUnproductiveDuration = FormatDuration(totalUnproductiveDuration);
                        dynamicItem.TotalNeutralDuration = FormatDuration(totalNeutralDuration);
                        dynamicItem.TotalDuration = FormatDuration(totalDurationInSeconds);
                        dynamicItem.PercentageProductiveDuration = percentageProductiveDuration;
                        dynamicItem.AverageProductiveDuration = FormatDuration(averageProductiveDuration);
                        dynamicItem.AverageUnproductiveDuration = FormatDuration(averageUnproductiveDuration);
                        dynamicItem.AverageNeutralDuration = FormatDuration(averageNeutralDuration);

                        result23.Add(dynamicItem);
                    }
                }
                foreach (var record in result23)
                {
                    var dynamicItem = new ExpandoObject() as dynamic;
                    dynamicItem.UserId = record.UserId;
                    dynamicItem.Date = record.Date;

                    // Check and add formatted durations and percentage
                    if (request.TotalProductivetime == true)
                        dynamicItem.Total_Productivetime = record.Total_Productivetime;

                    if (request.ProductivityPercent == true)
                        dynamicItem.Productivity_Percent = record.PercentageProductiveDuration;

                    if (request.AverageProductivetime == true)
                        dynamicItem.Average_Productivetime = record.AverageProductiveDuration;

                    if (request.Totalunproductivetime == true)
                        dynamicItem.Total_unproductivetime = record.TotalUnproductiveDuration;

                    if (request.Averageunproductivetime == true)
                        dynamicItem.Average_unproductivetime = record.AverageUnproductiveDuration;

                    if (request.Totalneutraltime == true)
                        dynamicItem.Total_neutraltime = record.TotalNeutralDuration;

                    if (request.Averageneutraltime == true)
                        dynamicItem.Average_neutraltime = record.AverageNeutralDuration;

                    result2.Add(dynamicItem);
                }

                var cc = result.Concat(result2);
                var allKeys = cc
                     .SelectMany(item => ((IDictionary<string, object>)item).Keys)
                     .Distinct()
                     .ToList();

                // Clear the existing list if it has any previous values
                groupedUsages.Clear();

                // Group and aggregate data
                groupedUsages.AddRange(cc
                    .GroupBy(u => new { u.Date, u.UserId }) // Group by Date and UserId
                    .Select(g =>
                    {
                        // Initialize a dictionary dynamically with default values for all keys
                        var aggregatedRecord = new ExpandoObject() as IDictionary<string, object>;
                        aggregatedRecord["Date"] = g.Key.Date;
                        aggregatedRecord["UserId"] = g.Key.UserId;

                        // Set default values for all keys
                        foreach (var key in allKeys)
                        {
                            if (key != "Date" && key != "UserId") // Skip grouping keys
                            {
                                aggregatedRecord[key] = 0; // Default value
                            }
                        }

                        // Merge properties from each item in the group
                        foreach (var item in g)
                        {
                            foreach (var prop in (IDictionary<string, object>)item)
                            {
                                if (aggregatedRecord.ContainsKey(prop.Key))
                                {
                                    aggregatedRecord[prop.Key] = prop.Value; // Update value if key exists
                                }
                            }
                        }

                        return new { Records = aggregatedRecord };
                    }));
            }
            else
            {
                foreach (var team in teams)
                {
                    var TeamName = team.TeamName;
                    var teamId = team.TeamId;

                    var urlUsageQuery = "Datewise_Activity1";
                    var parameters1 = new
                    {
                        OrganizationId = request.OrganizationId,
                        TeamId = teamId,
                        UserId = request.UserId,
                        FromDate = request.StartDate,
                        ToDate = request.EndDate
                    };

                    IEnumerable<dynamic> results = await _dapper.GetAllAsync<dynamic>(urlUsageQuery, parameters1);
                //    var getUsers = @"
                //SELECT id AS UserId, CONCAT(First_Name, ' ', Last_Name) AS FullName 
                //FROM Users 
                //WHERE TeamId = @TeamId
                //AND (@UserId IS NULL OR Id = @UserId)";

                //    var getUsers1 = await _dapper.GetAllAsync<dynamic>(getUsers, parameters1);

                //    var combinedResults = results.Concat(getUsers1)
                //        .GroupBy(item => item.UserId)
                //        .Select(group =>
                //        {
                //            var usageEntry = results.FirstOrDefault(u => u.UserId == group.Key);
                //            return usageEntry ?? group.First();
                //        })
                //        .ToList();

                    foreach (var us in results)
                    {
                        var userIdd = us.UserId;
                        var FullName = us.FullName;
                        var StartDate = us.Date;
                        var EndDate = us.Date;
                        int totalProductiveDuration = 0, totalUnproductiveDuration = 0, totalNeutralDuration = 0;

                        var usages = await GetAppUsages(request.OrganizationId, teamId, userIdd, StartDate, EndDate);

                        foreach (var usage in usages)
                        {
                            var totalSeconds = usage.TotalSeconds;
                            usage.ApplicationName = usage.ApplicationName.ToLower();

                            if (usage.ApplicationName != "chrome" && usage.ApplicationName != "msedge" && usage.ApplicationName != "firefox" && usage.ApplicationName != "opera")
                            {

                                usage.TotalSeconds = totalSeconds;
                                usage.TotalUsage = TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");



                                var parameters2 = new { ApplicationName = usage.ApplicationName.ToLower() };
                                var app = await _dapper.QueryFirstOrDefaultAsync<string>("GetApplicationCategoryAndProductivity", parameters2, commandType: CommandType.StoredProcedure);

                                if (app != null)
                                {
                                    switch (app)
                                    {
                                        case "Productive":
                                            totalProductiveDuration += usage.TotalSeconds;
                                            break;
                                        case "Unproductive":
                                            totalUnproductiveDuration += usage.TotalSeconds;
                                            break;
                                        case "Neutral":
                                            totalNeutralDuration += usage.TotalSeconds;
                                            break;
                                    }
                                }
                            }
                        }

                        var AttendanceCount = us.AttendanceCount;
                        var onlineDurationInSeconds = us.OnlineDurationInHours ?? 0.0;
                        double? activeDurationInSeconds = us.ActiveTimeInSeconds ?? 0.0;
                        var breakDurationInSeconds = us.TotalBreakDurationInSeconds ?? 0.0;
                        var totalDurationInSeconds = totalProductiveDuration + totalUnproductiveDuration + totalNeutralDuration;

                        var dateDifferenceInDays = (request.EndDate - request.StartDate).TotalDays;
                        if (dateDifferenceInDays <= 0)
                        {
                            dateDifferenceInDays = 1;
                        }

                        var percentageProductiveDuration = activeDurationInSeconds > 0 ? ((double)totalProductiveDuration / activeDurationInSeconds.Value) * 100 : 0.0;
                        double averageProductiveDuration = dateDifferenceInDays > 0 ? (double)totalProductiveDuration / dateDifferenceInDays : 0.0;
                        double averageUnproductiveDuration = dateDifferenceInDays > 0 ? (double)totalUnproductiveDuration / dateDifferenceInDays : 0.0;
                        double averageNeutralDuration = dateDifferenceInDays > 0 ? (double)totalNeutralDuration / dateDifferenceInDays : 0.0;

                        string FormatDuration(double totalSeconds)
                        {
                            var hours = (long)(totalSeconds / 3600);
                            var minutes = (long)((totalSeconds % 3600) / 60);
                            var seconds = (long)(totalSeconds % 60);
                            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
                        }

                        var dynamicItem = new ExpandoObject() as dynamic;
                        dynamicItem.UserId = userIdd;
                        dynamicItem.Date = StartDate;
                        //dynamicItem.Full_Name = FullName;
                        //dynamicItem.Team_Name = TeamName;
                        //dynamicItem.AttendanceCount = AttendanceCount;

                        //dynamicItem.ActiveDuration = FormatDuration(activeDurationInSeconds ?? 0.0);
                        //dynamicItem.BreakDuration = FormatDuration(breakDurationInSeconds);
                        //dynamicItem.OnlineDuration = FormatDuration(onlineDurationInSeconds);
                        dynamicItem.Total_Productivetime = FormatDuration(totalProductiveDuration);
                        dynamicItem.TotalUnproductiveDuration = FormatDuration(totalUnproductiveDuration);
                        dynamicItem.TotalNeutralDuration = FormatDuration(totalNeutralDuration);
                        dynamicItem.TotalDuration = FormatDuration(totalDurationInSeconds);
                        dynamicItem.PercentageProductiveDuration = percentageProductiveDuration;
                        dynamicItem.AverageProductiveDuration = FormatDuration(averageProductiveDuration);
                        dynamicItem.AverageUnproductiveDuration = FormatDuration(averageUnproductiveDuration);
                        dynamicItem.AverageNeutralDuration = FormatDuration(averageNeutralDuration);

                        result23.Add(dynamicItem);
                    }

                }
                foreach (var record in result23)
                {
                    var dynamicItem = new ExpandoObject() as dynamic;
                    dynamicItem.UserId = record.UserId;
                    dynamicItem.Date = record.Date;
                    // Check and add formatted durations and percentage
                    if (request.TotalProductivetime == true)
                        dynamicItem.Total_Productivetime = record.Total_Productivetime;

                    if (request.ProductivityPercent == true)
                        dynamicItem.Productivity_Percent = record.PercentageProductiveDuration;

                    if (request.AverageProductivetime == true)
                        dynamicItem.Average_Productivetime = record.AverageProductiveDuration;

                    if (request.Totalunproductivetime == true)
                        dynamicItem.Total_unproductivetime = record.TotalUnproductiveDuration;

                    if (request.Averageunproductivetime == true)
                        dynamicItem.Average_unproductivetime = record.AverageUnproductiveDuration;

                    if (request.Totalneutraltime == true)
                        dynamicItem.Total_neutraltime = record.TotalNeutralDuration;

                    if (request.Averageneutraltime == true)
                        dynamicItem.Average_neutraltime = record.AverageNeutralDuration;

                    result2.Add(dynamicItem);
                    result.Add(dynamicItem);
                }
                var cc = result.Concat(result2);
                // Determine all unique property names dynamically
                var allKeys = cc
                    .SelectMany(item => ((IDictionary<string, object>)item).Keys)
                    .Distinct()
                    .ToList();

                // Clear the existing list if it has any previous values
                groupedUsages.Clear();

                // Group and aggregate data
                groupedUsages.AddRange(cc
                    .GroupBy(u => new { u.Date, u.UserId }) // Group by Date and UserId
                    .Select(g =>
                    {
                        // Initialize a dictionary dynamically with default values for all keys
                        var aggregatedRecord = new ExpandoObject() as IDictionary<string, object>;
                        aggregatedRecord["Date"] = g.Key.Date;
                        aggregatedRecord["UserId"] = g.Key.UserId;

                        // Set default values for all keys
                        foreach (var key in allKeys)
                        {
                            if (key != "Date" && key != "UserId") // Skip grouping keys
                            {
                                aggregatedRecord[key] = 0; // Default value
                            }
                        }

                        // Merge properties from each item in the group
                        foreach (var item in g)
                        {
                            foreach (var prop in (IDictionary<string, object>)item)
                            {
                                if (aggregatedRecord.ContainsKey(prop.Key))
                                {
                                    aggregatedRecord[prop.Key] = prop.Value; // Update value if key exists
                                }
                            }
                        }

                        return new { Records = aggregatedRecord };
                    }));
            }

            return groupedUsages;
        }
    }
}
