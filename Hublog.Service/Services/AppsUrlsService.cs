using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.ApplicationModel;
using Hublog.Repository.Entities.Model.UrlModel;
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

        public async Task<List<GetUrlUsage>> GetUsersUrlUsages(int organizationId, int? teamid, int? userId, DateTime startDate, DateTime endDate)
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

        public async Task<TopUrlUsageResponse> GetTopUrlUsageAsync(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate)
        {
            var result = await _appsUrlsRepository.GetTopUrlUsageAsync(organizationId, teamId, userId, startDate, endDate);
            return new TopUrlUsageResponse
            {
                Url = result.Url,
                MaxUsage = result.MaxUsage
            };
        }

        public async Task<TopAppUsageResponse> GetTopAppUsageAsync(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate)
        {
            var result = await _appsUrlsRepository.GetTopAppUsageAsync(organizationId, teamId, userId, startDate, endDate);
            return new TopAppUsageResponse
            {
                ApplicationName = result.ApplicationName,
                MaxUsage = result.MaxUsage
            };
        }
        public async Task<TopAppUsageResponse> GetTopCategory(int organizationId, int? teamId, int? userId, DateTime startDate, DateTime endDate)
        {
            var result = await _appsUrlsRepository.GetTopCategory(organizationId, teamId, userId, startDate, endDate);
            return new TopAppUsageResponse
            {
                ApplicationName = result.ApplicationName,
                MaxUsage = result.MaxUsage
            };
        }

        public async Task InsertDefaultRecordsAsync(int organizationId)
        {
            try
            {
                await _appsUrlsRepository.InsertDefaultRecordsAsync(organizationId);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            
        }
    }
}
