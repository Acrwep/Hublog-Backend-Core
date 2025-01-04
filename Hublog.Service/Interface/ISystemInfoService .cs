using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface ISystemInfoService
    {
        Task<bool> InsertOrUpdateSystemInfo(SystemInfoModel systemInfoModel);

        Task<object> GetSystemInfo(int organizationId, int? userid, int? teamId, string userSearchQuery, string platformSearchQuery, string systemTypeSearchQuery);

        Task<UserStatistics> GetSystemInfoCount(int organizationId, int? teamId, int? userId, string userSearchQuery, string platformSearchQuery, string systemTypeSearchQuery);
        Task<IEnumerable<Hublog.Repository.Entities.Model.Version>> GetHublogVersion();
    }
}
