using Dapper;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Login;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace Hublog.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Dapperr _dapper;
        public UserRepository(Dapperr dapper)
        {
            _dapper = dapper;   
        }

        #region InsertBreak
        public async Task<ResultModel> InsertBreak(List<UserBreakModel> userBreakModels)
        {
            try
            {
                string details = JsonConvert.SerializeObject(userBreakModels);
                details = details.Replace("\"null\"", "\"\"");
                details = details.Replace("null", "\"\"");
                details = details.Replace("'", "");

                var formattedDetails = new List<dynamic>();
                foreach (var item in userBreakModels)
                {
                    formattedDetails.Add(new
                    {
                        item.OrganizationId,
                        item.BreakEntryId,
                        item.Id,
                        item.UserId,
                        BreakDate = item.BreakDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        Start_Time = item.Start_Time.ToString("yyyy-MM-dd HH:mm:ss"),
                        End_Time = item.End_Time?.ToString("yyyy-MM-dd HH:mm:ss"),
                        item.Status
                    });
                }
                details = JsonConvert.SerializeObject(formattedDetails);

                var parameters = new { details };
                var result = await _dapper.GetAsync<ResultModel>("Exec [SP_BreakEntry] @details", parameters);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting break: {ex.Message}");
                return new ResultModel { Result = 0, Msg = ex.Message };
            }
        }
        #endregion

        #region InsertAttendance
        public async Task<int> InsertAttendanceAsync(UserAttendanceModel model)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@OrganizationId", model.OrganizationId);
                parameters.Add("@AttendanceDate", model.AttendanceDate.ToString("yyyy-MM-dd HH:mm:ss"));
                parameters.Add("@Start_Time", model.Start_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                parameters.Add("@End_Time", model.End_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                parameters.Add("@Total_Time", null);
                parameters.Add("@Late_Time", null);
                parameters.Add("@Status", model.Status);

                var result = await _dapper.ExecuteAsync("SP_InsertAttendance", parameters, CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting attendance: {ex.Message}");
                return 0;
            }
        }
        #endregion

        #region SaveUserScreenShot
        public async Task SaveUserScreenShot(UserScreenShot userScreenShot)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userScreenShot.UserId);
            parameters.Add("@OrganizationId", userScreenShot.OrganizationId);
            parameters.Add("@ScreenShotDate", userScreenShot.ScreenShotDate);
            parameters.Add("@FileName", userScreenShot.FileName);
            parameters.Add("@ImageData", userScreenShot.ImageData);

            await _dapper.ExecuteAsync("SP_InsertScreenShot", parameters, CommandType.StoredProcedure);
        }
        #endregion

        #region GetUserAttendanceDetails
        public async Task<List<UserAttendanceDetailModel>> GetUserAttendanceDetails(int userId, DateTime startDate, DateTime endDate)   
        {
            var query = @"
            SELECT 
                U.First_Name AS FirstName, 
                U.Email, 
                U.EmployeeID AS EmployeeId, 
                U.Active, 
                A.AttendanceDate, 
                A.Start_Time, 
                A.End_Time, 
                A.Total_Time, 
                A.Late_Time, 
                A.Status 
            FROM Users U
                INNER JOIN Attendance A ON U.Id = A.UserId
                WHERE U.Id = @UserId
                  AND A.AttendanceDate BETWEEN @StartDate AND @EndDate";

            var parameters = new { UserId = userId, StartDate = startDate, EndDate = endDate };

            return await _dapper.GetAllAsync<UserAttendanceDetailModel>(query, parameters);
        }
        #endregion

        #region GetUserBreakRecordDetails
        public async Task<List<UserBreakRecordModel>> GetUserBreakRecordDetails(int userId, DateTime startDate, DateTime endDate)
        {
            var query = @"
                    SELECT 
                        BE.UserId, 
                        BE.OrganizationId, 
                        BE.BreakDate, 
                        BE.Start_Time, 
                        BE.End_Time, 
                        BE.BreakEntryId, 
                        BE.Status,
                        BE.BreakDuration,
                        BM.Name as BreakType, 
                        BM.Active, 
                        BM.Max_Break_Time, 
                        U.First_Name as firstName, 
                        U.Email
                    FROM BreakEntry BE 
                    INNER JOIN BreakMaster BM ON BE.BreakEntryId = BM.Id
                    INNER JOIN Users U ON U.Id = BE.UserId
                    WHERE BE.UserId = @UserId
                    AND (
                        (BE.BreakDate BETWEEN @StartDate AND @EndDate)
                        OR (BE.Start_Time <= @EndDateTime AND BE.End_Time >= @StartDate)
                    )";

            var parameters = new
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate,
                EndDateTime = endDate.AddDays(1)
            };

            return await _dapper.GetAllAsync<UserBreakRecordModel>(query, parameters);
        }
        #endregion

        #region GetUsersByTeamId
        public async Task<List<Users>> GetUsersByTeamId(int teamId) 
        {
            var query = @"
            SELECT 
                u.[Id],
                u.[First_Name],
                u.[Last_Name],
                u.[Email],
                u.[DOB],
                u.[DOJ],
                u.[Phone],
                u.[UsersName],
                u.[Password],
                u.[Gender],
                u.[OrganizationId],
                u.[RoleId],
                u.[DesignationId],
                u.[TeamId],
                u.[Active],
                u.[EmployeeID],
                t.[Name] AS TeamName
            FROM 
                [dbo].[Users] u
            INNER JOIN 
                [dbo].[Team] t
            ON 
                u.[TeamId] = t.[Id]
            WHERE 
                u.[TeamId] = @TeamId
        "
            ;

            return await _dapper.GetAllAsync<Users>(query, new { TeamId = teamId });
        }
        #endregion

        #region GetUsersByOrganizationId
        public async Task<List<Users>> GetUsersByOrganizationId(int organizationId)
        {
            try
            {
                string query = @"SELECT * FROM Users WHERE OrganizationId = @OrganizationId AND Active = 1";
                return await _dapper.GetAllAsync<Users>(query, new { OrganizationId = organizationId });
            }
            catch(Exception ex)
            {
                throw new Exception("Error fetching the data", ex);
            }
        }
        #endregion

        #region GetAvailableBreak
        public async Task<List<BreakMaster>> GetAvailableBreak(int organizationId, DateTime CDate, int userId)
        {
            string sqlQuery = @"
            SELECT * 
            FROM BreakMaster B WITH (NOLOCK)
            WHERE B.OrganizationId = @OrganizationId
              AND B.Active = 1
              AND B.Id NOT IN (
                  SELECT DISTINCT Id
                  FROM BreakEntry BE
                  WHERE BE.Start_Time = @CDate
                    AND BE.UserId = @UserId
                    AND BE.OrganizationId = @OrganizationId
            )";

            var parameters = new DynamicParameters();
            parameters.Add("@OrganizationId", organizationId);
            parameters.Add("@CDate", CDate);
            parameters.Add("@UserId", userId);

            var result = await _dapper.GetAllAsync<BreakMaster>(sqlQuery, parameters);
            return result;
        }
        #endregion

        #region GetBreakMasterById
        public async Task<BreakMaster> GetBreakMasterById(int id)
        {
            try
            {
                var query = @"SELECT * FROM BreakMaster WHERE Id = @Id";
                var parameters = new { Id = id };
                return await _dapper.GetAsync<BreakMaster>(query, parameters);

            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}", ex);
            }
        }
        #endregion

        #region GetAllUser
        public async Task<List<Users>> GetAllUser() 
        {
            var query = "SELECT * FROM Users WITH (NOLOCK)";
            return await _dapper.GetAllAsync<Users>(query);
        }
        #endregion

        #region InsertUser
        public async Task<int> InsertUser(Users user)
        {
            string query = @"
            INSERT INTO Users (First_Name, Last_Name, Email, DOB, DOJ, Phone, UsersName, Password, 
            Gender, OrganizationId, RoleId, DesignationId, TeamId, Active, EmployeeID) 
            VALUES (@First_Name, @Last_Name, @Email, @DOB, @DOJ, @Phone, @UsersName, @Password,
            @Gender, @OrganizationId, @RoleId, @DesignationId, @TeamId, @Active, @EmployeeID);
            SELECT CAST(SCOPE_IDENTITY() as int)";

            user.Active = true;

            return await _dapper.ExecuteAsync(query, user);
        }
        #endregion

        #region UpdateUser
        public async Task<int> UpdateUser(Users user)
        {
            string query = @"
            UPDATE Users 
            SET First_Name = @First_Name, Last_Name = @Last_Name, Email = @Email, DOB = @DOB, DOJ = @DOJ, Phone = @Phone, 
                UsersName = @UsersName, Password = @Password, Gender = @Gender, OrganizationId = @OrganizationId, 
                RoleId = @RoleId, DesignationId = @DesignationId, TeamId = @TeamId, Active = @Active, EmployeeID = @EmployeeID 
            WHERE Id = @Id";

            return await _dapper.ExecuteAsync(query, user);
        }
        #endregion

        #region DeleteUser
        public async Task<int> DeleteUser(int userId)
        {
            var query = @"DELETE FROM Users WHERE Id = @Id";
            var parameter = new { Id = userId };

            return await _dapper.ExecuteAsync(query, parameter);
        }
        #endregion 
    }
}
