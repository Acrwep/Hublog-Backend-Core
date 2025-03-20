using Hublog.Repository.Common;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Hublog.Repository.Repositories
{
    public class SystemInfoRepository : ISystemInfoRepository
    {
        private readonly Dapperr _dapper;
        public SystemInfoRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }
        public async Task<SystemInfoModel> GetSystemInfoByUserIdAndDeviceId(int userId, string deviceId)
        {
            var sql = "SELECT * FROM SystemInfo WHERE UserId = @UserId AND DeviceId = @DeviceId";
            return await _dapper.QueryFirstOrDefaultAsync<SystemInfoModel>(sql, new { UserId = userId, DeviceId = deviceId });
        }

        public async Task<SystemInfoModel> InsertSystemInfo(SystemInfoModel systemInfoModel)
        {
            var sql = @"
                    INSERT INTO SystemInfo (UserId, DeviceId, DeviceName, Platform, OSName, OSBuild, SystemType, IPAddress, AppType, HublogVersion, Status) 
                    VALUES (@UserId, @DeviceId, @DeviceName, @Platform, @OSName, @OSBuild, @SystemType, @IPAddress, @AppType, @HublogVersion, @Status);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var id = await _dapper.ExecuteScalarAsync<int>(sql, systemInfoModel);
            systemInfoModel.Id = id; 
            return systemInfoModel;
        }

        public async Task UpdateSystemInfo(SystemInfoModel systemInfoModel)   
        {
            var sql = @"
                    UPDATE SystemInfo 
                    SET 
                        UserId = @UserId,
                        DeviceId =@DeviceId,
                        DeviceName = @DeviceName,
                        Platform = @Platform,
                        OSName = @OSName,
                        OSBuild = @OSBuild,
                        SystemType =@SystemType,
                        IPAddress = @IPAddress,
                        AppType = @AppType,
                        HublogVersion =@HublogVersion,
                        Status = @Status
                    WHERE Id = @Id;";

           await _dapper.ExecuteAsync(sql, systemInfoModel);          
        }

        public async Task<object> GetSystemInfo(int organizationId, int? userid, int? teamId, string userSearchQuery, string platformSearchQuery, string systemTypeSearchQuery)
        {
            var query = "EXEC GetSystemInfo @organizationId, @userid, @teamId, @userSearchQuery, @platformSearchQuery, @systemTypeSearchQuery";

            var parameters = new
            {
                organizationId,
                userid = userid,
                teamId,
                userSearchQuery = userSearchQuery ?? string.Empty,
                platformSearchQuery = platformSearchQuery ?? string.Empty,
                systemTypeSearchQuery = systemTypeSearchQuery ?? string.Empty
            };

            var systemInfoList = await _dapper.GetAllAsync<SystemInfoDto>(query, parameters);

            int onlineCount = systemInfoList.Count(info => int.TryParse(info.Status, out var status) && status == 1);
            int offlineCount = systemInfoList.Count(info => int.TryParse(info.Status, out var status) && status == 0);
            int winUICount = systemInfoList.Count(info => info.Platform.Contains("WinUI"));
            int macCount = systemInfoList.Count(info => info.Platform.Contains("Mac"));
            int linuxCount = systemInfoList.Count(info => info.Platform.Contains("Linux"));

            var aggregateCounts = new
            {
                OnlineCount = onlineCount,
                OfflineCount = offlineCount,
                WinUICount = winUICount,
                MacCount = macCount,
                LinuxCount = linuxCount
            };

            return new
            {
                AggregateCounts = aggregateCounts,
                SystemInfoList = systemInfoList
            };
        }


        public async Task<UserStatistics> GetSystemInfoCount(int organizationId, int? teamId, int? userId, string userSearchQuery, string platformSearchQuery, string systemTypeSearchQuery)
        {
            var query = @"
                SELECT 
                    SUM(CASE WHEN SY.Status = 1 AND SY.SystemType LIKE '%' + @SystemTypeSearchQuery + '%' AND SY.Platform LIKE '%' + @PlatformSearchQuery + '%' THEN 1 ELSE 0 END) AS OnlineCount,
                    SUM(CASE WHEN SY.Status = 0 AND SY.SystemType LIKE '%' + @SystemTypeSearchQuery + '%' AND SY.Platform LIKE '%' + @PlatformSearchQuery + '%' THEN 1 ELSE 0 END) AS OfflineCount,
                    SUM(CASE WHEN SY.Platform LIKE '%WinUI%' THEN 1 ELSE 0 END) AS WinUICount,
                    SUM(CASE WHEN SY.Platform LIKE '%Mac%' THEN 1 ELSE 0 END) AS MacCount,
                    SUM(CASE WHEN SY.Platform LIKE '%Linux%' THEN 1 ELSE 0 END) AS LinuxCount
                FROM 
                    Users U
                INNER JOIN 
                    SystemInfo SY ON U.Id = SY.UserId
                INNER JOIN 
                    Team T ON T.Id = U.TeamId
                INNER JOIN 
                    Organization O ON U.OrganizationId = O.Id
                WHERE 
                    O.Id = @OrganizationId  
                    AND (@TeamId IS NULL OR T.Id = @TeamId)  
                    AND (@UserId IS NULL OR U.Id = @UserId)
                    AND (@UserSearchQuery = '' OR CONCAT(U.First_Name, ' ', U.Last_Name) LIKE '%' + @UserSearchQuery + '%')
                    AND (@PlatformSearchQuery = '' OR SY.Platform LIKE '%' + @PlatformSearchQuery + '%')
                    AND (@SystemTypeSearchQuery = '' OR SY.SystemType LIKE '%' + @SystemTypeSearchQuery + '%')";

            var parameters = new
            {
                OrganizationId = organizationId,
                TeamId = teamId,
                UserId = userId,
                UserSearchQuery = userSearchQuery,
                PlatformSearchQuery = platformSearchQuery,
                SystemTypeSearchQuery = systemTypeSearchQuery
            };

            return await _dapper.QueryFirstOrDefaultAsync<UserStatistics>(query, parameters);
        }
        public async Task<IEnumerable<Hublog.Repository.Entities.Model.Version>> GetHublogVersion()
        {
            string query = "SELECT id, versionNumber, DownloadUrl FROM HublogVersion";
            return await _dapper.GetAllAsync<Entities.Model.Version>(query);
        }


    }
}
