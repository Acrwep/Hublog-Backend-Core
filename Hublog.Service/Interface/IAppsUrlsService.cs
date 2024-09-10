using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface IAppsUrlsService
    {
        //Task TrackApplicationUsage(int userId, string applicationName, string totalUsage, string details, DateTime usageDate);//, string url);

        Task<List<GetApplicationUsage>> GetUsersApplicationUsages(int organizationId, int userId, DateTime startDate, DateTime endDate);


        Task<bool> LogApplicationUsageAsync(ApplicationUsage applicationUsage);

        Task<bool> LogUrlUsageAsync(UrlUsage urlUsage);
    }
}
