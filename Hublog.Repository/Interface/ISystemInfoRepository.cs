using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;

namespace Hublog.Repository.Interface
{
    public interface ISystemInfoRepository
    {
        Task<SystemInfoModel> GetSystemInfoByUserIdAndDeviceId(int userId, string deviceId);
        Task<SystemInfoModel> InsertSystemInfo(SystemInfoModel systemInfoModel);
        Task UpdateSystemInfo(SystemInfoModel systemInfoModel);

        Task<List<SystemInfoDto>> GetSystemInfo(int organizationId, int? teamId, string userSearchQuery, string platformSearchQuery, string systemTypeSearchQuery);
    }
}
