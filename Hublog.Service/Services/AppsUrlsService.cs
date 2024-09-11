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

        public async Task<List<GetApplicationUsage>> GetUsersApplicationUsages(int organizationId,int? teamid, int? userId, DateTime startDate, DateTime endDate)
        {
            return await _appsUrlsRepository.GetUsersApplicationUsages(organizationId, teamid, userId, startDate, endDate);
        }

        public async Task<List<GetApplicationUsage>> GetUsersUrlUsages(int organizationId, int? teamid, int? userId, DateTime startDate, DateTime endDate)
        {
            return await _appsUrlsRepository.GetUsersUrlUsages(organizationId, teamid, userId, startDate, endDate);
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
