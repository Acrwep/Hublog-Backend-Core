using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.ApplicationModel;
using Hublog.Repository.Entities.Model.UrlModel;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Repository.Interface
{
    public interface IAppsUrlsRepository
    {
        Task<List<GetApplicationUsage>> GetUsersApplicationUsages(int organizationId, int? teamid, int? userId, DateTime startDate, DateTime endDate);

        Task<List<GetUrlUsage>> GetUsersUrlUsages(int organizationId, int? teamid, int? userId, DateTime startDate, DateTime endDate);  

        Task<int> InsertApplicationUsageAsync(ApplicationUsage applicationUsage);

        Task<int> InsertUrlUsageAsync(UrlUsage urlUsage);

        Task<(string Url, string MaxUsage)> GetTopUrlUsageAsync(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate);

        Task<(string ApplicationName, string MaxUsage)> GetTopAppUsageAsync(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate);
        Task<(string ApplicationName, string MaxUsage)> GetTopCategory(int organizationId, int? teamId, int? userId, DateTime fromDate, DateTime toDate);

        Task InsertDefaultRecordsAsync(int organizationId);
    }
}
