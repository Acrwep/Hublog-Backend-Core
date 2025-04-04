﻿using Hublog.Repository.Entities.Model.DashboardModel;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Service.Interface
{
    public interface IAttendanceDashboardService
    {
        Task<List<UserAttendanceReport>> GetUserTotalAttendanceAndBreakSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate);

        Task<object> GetAllAttendanceSummary(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate);

        Task<AttendanceDashboardSummaryModel> AttendanceDashboardSummary(int organizationId, int? teamId, DateTime startDate, DateTime endDate);
        Task<object> BreakTrends([FromQuery] int organizationId, [FromQuery] int? teamId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate);

        Task<List<TeamProductivityModel>> GetTopTeamProductivity(int organizationId, int? teamId, DateTime startDate, DateTime endDate);

        Task<List<TeamProductivityModel>> GetLeastTeamProductivity(int organizationId, int? teamId, DateTime startDate, DateTime endDate);

        Task<object> GetLateArrivals(int organizationId, int? teamId, DateTime startDate, DateTime endDate);
    }
}
