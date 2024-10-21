﻿using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.ApplicationModel;
using Hublog.Repository.Entities.Model.UrlModel;

namespace Hublog.Service.Interface
{
    public interface IAppsUrlsService
    {
        Task<List<GetApplicationUsage>> GetUsersApplicationUsages(int organizationId,int? teamid, int? userId, DateTime startDate, DateTime endDate);

        Task<List<GetUrlUsage>> GetUsersUrlUsages(int organizationId, int? teamid, int? userId, DateTime startDate, DateTime endDate);

        Task<bool> LogApplicationUsageAsync(ApplicationUsage applicationUsage);

        Task<bool> LogUrlUsageAsync(UrlUsage urlUsage);

        Task<TopUrlUsageResponse> GetTopUrlUsageAsync(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate);

        Task<TopAppUsageResponse> GetTopAppUsageAsync(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate);
    }
}
