using Hublog.Repository.Common;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;

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
                    SET Status = @Status 
                    WHERE Id = @Id;";

            await _dapper.ExecuteAsync(sql, systemInfoModel);
        }

        public async Task<List<SystemInfoDto>> GetSystemInfo(int organizationId, int? teamId, string userSearchQuery, string platformSearchQuery, string systemTypeSearchQuery)
        {
            var query = @"
        SELECT
            CONCAT(U.First_Name, ' ', U.Last_Name) AS Full_Name,
            SY.DeviceId,
            SY.DeviceName,
            SY.Platform,
            SY.OSName,
            SY.OSBuild,
            SY.SystemType,
            SY.IPAddress,
            SY.AppType,
            SY.HublogVersion,
            SY.Status,
            T.Id AS TeamId
        FROM 
            Users U
        INNER JOIN 
            SystemInfo SY ON U.Id = SY.UserId
        INNER JOIN 
            Team T ON T.Id = U.TeamId
        INNER JOIN 
            Organization O ON U.OrganizationId = O.Id
        WHERE 
            O.Id = @organizationId  
            AND (@teamId IS NULL OR T.Id = @teamId)  
            AND ( 
                (@userSearchQuery IS NULL OR @userSearchQuery = '' OR CONCAT(U.First_Name, ' ', U.Last_Name) LIKE '%' + @userSearchQuery + '%')
            )
            AND (@platformSearchQuery IS NULL OR @platformSearchQuery = '' OR SY.Platform LIKE '%' + @platformSearchQuery + '%')
            AND (@systemTypeSearchQuery IS NULL OR @systemTypeSearchQuery = '' OR SY.SystemType LIKE '%' + @systemTypeSearchQuery + '%');
    ";

            var parameters = new
            {
                organizationId,
                teamId,
                userSearchQuery = userSearchQuery ?? string.Empty,
                platformSearchQuery = platformSearchQuery ?? string.Empty,
                systemTypeSearchQuery = systemTypeSearchQuery ?? string.Empty
            };

            return await _dapper.GetAllAsync<SystemInfoDto>(query, parameters);
        }

    }
}
