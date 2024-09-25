using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface ISystemInfoService
    {
        Task<bool> InsertOrUpdateSystemInfo(SystemInfoModel systemInfoModel);

        Task<List<SystemInfoDto>> GetSystemInfo(int organizationId, int? userid, int? teamId, string userSearchQuery, string platformSearchQuery, string systemTypeSearchQuery);
    }
}
