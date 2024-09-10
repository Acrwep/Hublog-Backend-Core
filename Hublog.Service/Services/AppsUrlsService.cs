using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class AppsUrlsService : IAppsUrlsService
    {
        private readonly IAppsUrlsRepository _appsUrlsRepository;
        public AppsUrlsService(IAppsUrlsRepository appsUrlsRepository)
        {
            _appsUrlsRepository = appsUrlsRepository;
        }

        //public async Task TrackApplicationUsage(int userId, string applicationName, string totalUsage, string details, DateTime usageDate)//, string url)
        //{
        //    try
        //    {
        //        await _appsUrlsRepository.TrackApplicationUsage(userId, applicationName, totalUsage, details, usageDate);//, url);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error in service while tracking application usage: {ex.Message}");
        //        throw; 
        //    }
        //}


        public async Task<List<GetApplicationUsage>> GetUsersApplicationUsages(int organizationId, int userId, DateTime startDate, DateTime endDate)
        {
            return await _appsUrlsRepository.GetUsersApplicationUsages(organizationId, userId, startDate, endDate);
        }


        public async Task<bool> LogApplicationUsageAsync(ApplicationUsage applicationUsage)
        {
            var result = await _appsUrlsRepository.InsertApplicationUsageAsync(applicationUsage);
            return result > 0;
        }

        public async Task<bool> LogUrlUsageAsync(UrlUsage urlUsage)
        {
            var result = await _appsUrlsRepository.InsertUrlUsageAsync(urlUsage);
            return result > 0;
        }
    }
}
