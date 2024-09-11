using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;

namespace Hublog.Repository.Interface
{
    public interface IAppsUrlsRepository
    {
        Task<List<GetApplicationUsage>> GetUsersApplicationUsages(int organizationId, int? teamid, int? userId, DateTime startDate, DateTime endDate);

        Task<List<GetApplicationUsage>> GetUsersUrlUsages(int organizationId, int? teamid, int? userId, DateTime startDate, DateTime endDate);  

        Task<int> InsertApplicationUsageAsync(ApplicationUsage applicationUsage);

        Task<int> InsertUrlUsageAsync(UrlUsage urlUsage);
    }
}
