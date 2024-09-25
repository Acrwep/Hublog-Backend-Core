using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
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

        public async Task<List<SystemInfoDto>> GetSystemInfo(int organizationId, int? userid, int? teamId, string userSearchQuery, string platformSearchQuery, string systemTypeSearchQuery)
        {
            return await _systemInfoRepository.GetSystemInfo(organizationId,userid, teamId, userSearchQuery, platformSearchQuery, systemTypeSearchQuery);
        }
    }
}
