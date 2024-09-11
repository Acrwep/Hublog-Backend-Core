using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface IAppsUrlsService
    {
        Task<List<GetApplicationUsage>> GetUsersApplicationUsages(int organizationId,int? teamid, int? userId, DateTime startDate, DateTime endDate);

        Task<List<GetApplicationUsage>> GetUsersUrlUsages(int organizationId, int? teamid, int? userId, DateTime startDate, DateTime endDate);

        Task<bool> LogApplicationUsageAsync(ApplicationUsage applicationUsage);

        Task<bool> LogUrlUsageAsync(UrlUsage urlUsage);
    }
}
