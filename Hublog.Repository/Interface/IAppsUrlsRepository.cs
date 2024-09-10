using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;

namespace Hublog.Repository.Interface
{
    public interface IAppsUrlsRepository
    {
        //Task TrackApplicationUsage(int userId, string applicationName, string totalUsage, string details, DateTime usageDate, string url);

        Task<List<GetApplicationUsage>> GetUsersApplicationUsages(int organizationId, int userId, DateTime startDate, DateTime endDate);


        Task<int> InsertApplicationUsageAsync(ApplicationUsage applicationUsage);

        Task<int> InsertUrlUsageAsync(UrlUsage urlUsage);
    }
}
