using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using Hublog.Repository.Repositories;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class SystemInfoService : ISystemInfoService
    {
        private readonly ISystemInfoRepository _systemInfoRepository;
        public SystemInfoService(ISystemInfoRepository systemInfoRepository)    
        {
            _systemInfoRepository = systemInfoRepository;
        }
        public async Task<bool> InsertOrUpdateSystemInfo(SystemInfoModel systemInfoModel)
        {
            var existingInfo = await _systemInfoRepository.GetSystemInfoByUserIdAndDeviceId(systemInfoModel.UserId, systemInfoModel.DeviceId);

            if (existingInfo != null)
            {
                existingInfo.Status = systemInfoModel.Status;

                await _systemInfoRepository.UpdateSystemInfo(existingInfo);
                return true; 
            }
            else 
            {
                await _systemInfoRepository.InsertSystemInfo(systemInfoModel);
                return true; 
            }
        }

        public async Task<object> GetSystemInfo(int organizationId, int? userid, int? teamId, string userSearchQuery, string platformSearchQuery, string systemTypeSearchQuery)
        {
            return await _systemInfoRepository.GetSystemInfo(organizationId,userid, teamId, userSearchQuery, platformSearchQuery, systemTypeSearchQuery);
        }

        public async Task<UserStatistics> GetSystemInfoCount(int organizationId, int? teamId, int? userId, string userSearchQuery, string platformSearchQuery, string systemTypeSearchQuery)
        {
            return await _systemInfoRepository.GetSystemInfoCount(organizationId, teamId, userId, userSearchQuery, platformSearchQuery, systemTypeSearchQuery);
        }
        public async Task<IEnumerable<Hublog.Repository.Entities.Model.Version>> GetHublogVersion()
        {
            return await _systemInfoRepository.GetHublogVersion();
        }
    }
}
