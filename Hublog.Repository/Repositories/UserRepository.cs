using Dapper;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Login;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;

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

                string details = JsonConvert.SerializeObject(formattedDetails);

                var parameters = new { details };
                var result = await _dapper.GetAsync<ResultModel>("Exec [SP_BreakEntry] @details", parameters);

                Console.WriteLine($"Stored Procedure Result Message: {result.Msg}");

                if (result != null && result.Msg.Contains("ongoing break"))
                {
                    Console.WriteLine("Entering ongoing break condition.");

                }

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
                string startTimeFormatted = model.Start_Time?.ToString("yyyy-MM-dd HH:mm:ss");
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@OrganizationId", model.OrganizationId);
                parameters.Add("@AttendanceDate", model.AttendanceDate.ToString("yyyy-MM-dd HH:mm:ss"));
                parameters.Add("@Start_Time", model.Start_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                parameters.Add("@Total_Time", null);
                parameters.Add("@Late_Time", null);
                parameters.Add("@Status", model.Status);
                if (model.Status == 1)
                {
                    if (model.End_Time != null)
                    {
                        parameters.Add("@End_Time", model.End_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        model.End_Time = DateTime.Now;
                        parameters.Add("@End_Time", model.End_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                if (model.Status == 0)
                {
                    parameters.Add("@End_Time", model.End_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                parameters.Add("@Punchout_type", model.Punchout_type); 
                Console.WriteLine(startTimeFormatted);
                var result = await _dapper.ExecuteAsync("[SP_InsertAttendance]", parameters, CommandType.StoredProcedure);// SP_PunchIn_InsertAttendance
                var deleteQuery = @"
                  DELETE UA
                  FROM UserActivity UA
                  INNER JOIN Attendance A ON A.UserId = UA.UserId
                  WHERE UA.TriggeredTime < DATEADD(DAY, -10, @AttendanceDate)
                  AND A.UserId = @UserId;
        ";

                var deleteResult = await _dapper.ExecuteAsync(deleteQuery, new { UserId = model.UserId, AttendanceDate = model.AttendanceDate }, CommandType.Text);



                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting attendance: {ex.Message}");
                return 0;
            }
        }
        #endregion
        public async Task PunchIn_InsertAttendance(List<UserAttendanceModel> models)
        {
            try
            {
                foreach (var model in models)
                {
                    var parameters = new DynamicParameters();
                    string startTimeFormatted = model.Start_Time?.ToString("yyyy-MM-dd HH:mm:ss");
                    parameters.Add("@UserId", model.UserId);
                    parameters.Add("@OrganizationId", model.OrganizationId);
                    parameters.Add("@AttendanceDate", model.AttendanceDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    parameters.Add("@Start_Time", model.Start_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                    parameters.Add("@Total_Time", null);
                    parameters.Add("@Late_Time", null);
                    parameters.Add("@Status", model.Status);

                    if (model.Status == 1)
                    {
                        if (model.End_Time != null)
                        {
                            parameters.Add("@End_Time", model.End_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else
                        {
                            model.End_Time = DateTime.Now;
                            parameters.Add("@End_Time", model.End_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                    }

                    if (model.Status == 0)
                    {
                        parameters.Add("@End_Time", model.End_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                    }

                    parameters.Add("@Punchout_type", model.Punchout_type);

                    await _dapper.ExecuteAsync("SP_PunchIn_InsertAttendance", parameters, CommandType.StoredProcedure);
                    var deleteQuery = @"
                  DELETE UA
                  FROM UserActivity UA
                  INNER JOIN Attendance A ON A.UserId = UA.UserId
                  WHERE UA.TriggeredTime < DATEADD(DAY, -10, @AttendanceDate)
                  AND A.UserId = @UserId; ";

                    var deleteResult = await _dapper.ExecuteAsync(deleteQuery, new { UserId = model.UserId, AttendanceDate = model.AttendanceDate }, CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting attendance: {ex.Message}");
                throw;
            }
        }

        public async Task PunchoutInsertAttendance(List<UserAttendanceModel> models)
            {
                try
                {
                    foreach (var model in models)
                    {
                        var parameters = new DynamicParameters();
                        string startTimeFormatted = model.Start_Time?.ToString("yyyy-MM-dd HH:mm:ss");
                        parameters.Add("@UserId", model.UserId);
                        parameters.Add("@OrganizationId", model.OrganizationId);
                        parameters.Add("@AttendanceDate", model.AttendanceDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        parameters.Add("@Start_Time", model.Start_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                        parameters.Add("@Total_Time", null);
                        parameters.Add("@Late_Time", null);
                        parameters.Add("@Status", model.Status);

                        if (model.Status == 1)
                        {
                            if (model.End_Time != null)
                            {
                                parameters.Add("@End_Time", model.End_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            else
                            {
                                model.End_Time = DateTime.Now;
                                parameters.Add("@End_Time", model.End_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                        }

                        if (model.Status == 0)
                        {
                            parameters.Add("@End_Time", model.End_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                        }

                        parameters.Add("@Punchout_type", model.Punchout_type);

                        await _dapper.ExecuteAsync("SP_PunchoutInsertAttendance", parameters, CommandType.StoredProcedure);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting attendance: {ex.Message}");
                    throw;
                }
            }


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
        public async Task<List<UserAttendanceDetailModel>> GetUserAttendanceDetails(int organizationId, int userId, DateTime startDate, DateTime endDate)   
        {
            var query = "GetAttendanceDetails";

            var parameters = new { OrganizationId = organizationId, UserId = userId, StartDate = startDate, EndDate = endDate };

            return await _dapper.GetAllAsync<UserAttendanceDetailModel>(query, parameters);
        }
        #endregion
        public async Task<List<UserAttendanceDetailModel>> GetUserPunchInOutDetails(int userId, int organizationId, DateTime startDate, DateTime endDate)
        {
            var query = "sp_GetUserPunchInOutDetails";
            var parameters = new
            {
                UserId = userId,
                OrganizationId = organizationId,
                StartDate = startDate,
                EndDate = endDate


            };

            return await _dapper.GetAllAsync<UserAttendanceDetailModel>(query, parameters);
        }
        public async Task<List<UserAttendanceDetailModel>> UpdateUserAttendanceDetails([FromBody] AttendanceUpdate request)
        {
            var query = "AttendanceDetails_Update"; 
            var parameters = new
            {
                UserId = request.UserId,
                OrganizationId = request.OrganizationId,
                Date = request.Date
            };

            var keys=await _dapper.GetAllAsync<UserAttendanceDetailModel>(query, parameters);
            return keys;
        }
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
                        RIGHT('00' + CAST(DATEDIFF(SECOND, BE.Start_Time, BE.End_Time) / 3600 AS VARCHAR(2)), 2) + ':' +
                        RIGHT('00' + CAST((DATEDIFF(SECOND, BE.Start_Time, BE.End_Time) % 3600) / 60 AS VARCHAR(2)), 2) + ':' +
                        RIGHT('00' + CAST(DATEDIFF(SECOND, BE.Start_Time, BE.End_Time) % 60 AS VARCHAR(2)), 2) AS BreakDuration,
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

            var result = await _dapper.GetAllAsync<UserBreakRecordModel>(query, parameters);

            // Ensure the BreakDuration is in the correct format (hh:mm:ss)
            foreach (var record in result)
            {
                if (!string.IsNullOrEmpty(record.BreakDuration) && record.BreakDuration.Contains(":"))
                {
                    // Ensure there are no invalid formats
                    var timeParts = record.BreakDuration.Split(':');
                    if (timeParts.Length == 3)
                    {
                        // Format to ensure "hh:mm:ss"
                        record.BreakDuration = string.Format("{0:D2}:{1:D2}:{2:D2}", int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]));
                    }
                }
            }

            return result;
        }
        #endregion

        #region GetUsersByTeamId
        public async Task<List<Users>> GetUsersByTeamId(int teamId) 
        {
            var query = @"
            SELECT 
                u.Id,
                u.First_Name,
                u.Last_Name,
                u.Email,
                u.DOB,
                u.DOJ,
                u.Phone,
                u.UsersName,
                u.Password,
                u.Gender,
                u.OrganizationId,
                u.RoleId,
                u.DesignationId,
                u.TeamId,
                u.Active,
                u.EmployeeID,
                t.Name AS TeamName
            FROM 
                Users u
            INNER JOIN 
                Team t
            ON 
                u.TeamId = t.Id
            WHERE 
                u.TeamId = @TeamId
                AND u.Active = 1
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
        public async Task<List<UsersDTO>> GetAllUser(string loggedInUserEmail, int organizationid, string searchQuery)
        {
            var query = @"
                        SELECT 
                            u.Id,
                            u.First_Name,
                            u.Last_Name,
                            CONCAT(U.First_Name, ' ', U.Last_Name) AS full_Name,
                            u.Email,
                            u.DOB,
                            u.DOJ,
                            u.Phone,
                            u.Password,
                            u.UsersName,
                            u.Gender,
                            u.OrganizationId,
                            u.RoleId,
                            u.DesignationId,
                            d.Name AS DesignationName,
                            u.TeamId,
                            t.Name AS TeamName,
                            u.Active,
                            u.EmployeeID
                        FROM Users u
                            LEFT JOIN Designation d ON u.DesignationId = d.Id
                            LEFT JOIN Team t ON u.TeamId = t.Id
                        WHERE u.OrganizationId = @OrganizationId
                        AND (u.First_Name LIKE @SearchQuery OR u.Email LIKE @SearchQuery)
                            ORDER BY CASE WHEN u.Email = @LoggedInUserEmail THEN 0 ELSE 1 END";

            var parameters = new
            {
                OrganizationId = organizationid,
                SearchQuery = $"%{searchQuery}%",
                LoggedInUserEmail = loggedInUserEmail
            };

            try
            {
                var users = await _dapper.GetAllAsync<UsersDTO>(query, parameters);
                return users ?? new List<UsersDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<UsersDTO>();
            }
        }

        #endregion

        #region GetActiveUsers
        public async Task<List<UsersDTO>> GetActiveUsers(string loggedInUserEmail, int organizationid, string searchQuery)
        {
            var query = @"
                        SELECT 
                            u.Id,
                            u.First_Name,
                            u.Last_Name,
                            CONCAT(U.First_Name, ' ', U.Last_Name) AS full_Name,
                            u.Email,
                            u.DOB,
                            u.DOJ,
                            u.Phone,
                            u.Password,
                            u.UsersName,
                            u.Gender,
                            u.OrganizationId,
                            u.RoleId,
                            u.DesignationId,
                            d.Name AS DesignationName,
                            u.TeamId,
                            t.Name AS TeamName,
                            u.Active,
                            u.EmployeeID
                        FROM Users u
                            LEFT JOIN Designation d ON u.DesignationId = d.Id
                            LEFT JOIN Team t ON u.TeamId = t.Id
                        WHERE u.OrganizationId = @OrganizationId
                        AND (u.First_Name LIKE @SearchQuery OR u.Email LIKE @SearchQuery)
                        AND u.Active = 1  -- Only active users
                            ORDER BY CASE WHEN u.Email = @LoggedInUserEmail THEN 0 ELSE 1 END";

            var parameters = new
            {
                OrganizationId = organizationid,
                SearchQuery = $"%{searchQuery}%",
                LoggedInUserEmail = loggedInUserEmail
            };

            try
            {
                var users = await _dapper.GetAllAsync<UsersDTO>(query, parameters);
                return users ?? new List<UsersDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<UsersDTO>();
            }
        }

        #endregion

        #region InsertUser
        public async Task<int> InsertUser(Users user)
        {
            string checkEmail = "SELECT * FROM Users WHERE Email = @Email";
            var AlreadyExistEmail = await _dapper.ExecuteScalarAsync<int>(checkEmail, new { user.Email });

            if (AlreadyExistEmail > 0)
            {
                return -1; 
            }

            string insertQuery = @"
                            INSERT INTO Users (First_Name, Last_Name, Email, DOB, DOJ, Phone, UsersName, Password, 
                                Gender, OrganizationId, RoleId, DesignationId, TeamId, Active, EmployeeID) 
                            VALUES (@First_Name, @Last_Name, @Email, @DOB, @DOJ, @Phone, @UsersName, @Password,
                                @Gender, @OrganizationId, @RoleId, @DesignationId, @TeamId, @Active, @EmployeeID);
                            SELECT CAST(SCOPE_IDENTITY() as int)";

            user.Active = true;

            return await _dapper.ExecuteAsync(insertQuery, user);
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

        #region TrackApplicationUsage
        public async Task TrackApplicationUsage(int userId, string applicationName, string totalUsage, string details, DateTime usageDate)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                parameters.Add("@ApplicationName", applicationName);
                parameters.Add("@TotalUsage", totalUsage);
                parameters.Add("@Details", details);
                parameters.Add("@UsageDate", usageDate);

                var result = await _dapper.ExecuteAsync("InsertOrUpdateApplicationUsage", parameters, commandType: CommandType.StoredProcedure);

                if (result <= 0)
                {
                    Console.WriteLine("Failed to log application usage.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error tracking application usage: {ex.Message}");
            }
        }
        #endregion

        public async Task<List<GetApplicationUsage>> GetUsersApplicationUsages(int organizationId, int userId, DateTime startDate, DateTime endDate)
        {
            string query = @"
                    SELECT 
                        au.Id AS ApplicationUsageId,
                        au.UserId,
                        u.Email,
                        u.OrganizationId,
                        au.ApplicationName,
                        au.TotalUsage,
                        au.Details,
                        au.CreatedDate,
                        au.UsageDate
                    FROM 
                        ApplicationUsage AS au
                    JOIN 
                        Users AS u ON au.UserId = u.Id
                    JOIN 
                        Organization AS o ON u.OrganizationId = o.Id
                    WHERE 
                        u.OrganizationId = @OrganizationId  
                        AND au.UserId = @UserId              
                        AND au.UsageDate BETWEEN @StartDate AND @EndDate";

            var parameter = new { organizationId, userId, startDate, endDate };

            return await _dapper.GetAllAsync<GetApplicationUsage>(query, parameter);
        }

        public async Task<List<UserTotalBreakModel>> GetUserTotalBreak(int organizationId, int userId, DateTime startDate, DateTime endDate)
        {
            var query = "GetUserTotalBreakHours";

            var parameter = new { OrganizationId = organizationId, 
                                  UserId = userId, 
                                  StartDate = startDate, 
                                  EndDate = endDate 
                                };

            return await _dapper.GetAllAsyncs<UserTotalBreakModel>(query, parameter, commandType: CommandType.StoredProcedure);
        }
        public async Task<UserActivity> Insert_Active_Time(UserActivity activity)
        {
            try
            {
                string query = @"INSERT INTO UserActivity (Userid, TriggeredTime)
                         VALUES (@Userid, @TriggeredTime);
                         SELECT CAST(SCOPE_IDENTITY() as int);";

                var createdBreakmaster = await _dapper.ExecuteAsync(query, activity);
                activity.Id = createdBreakmaster; 
                return activity;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating Breakmaster", ex);
            }
        }

        public async Task<List<UserActivity>> Get_Active_Time(int userid, DateTime startDate, DateTime endDate)
        {
            try
            {
                var query = @"
                           SELECT * 
                           FROM UserActivity
                           WHERE UserId = @UserId
                           AND TriggeredTime >= @StartDate
                           AND TriggeredTime < DATEADD(DAY, 1, @EndDate)";

                var parameters = new
                {
                    UserId = userid,
                    StartDate = startDate,
                    EndDate = endDate
                };

                var result=await _dapper.GetAllAsync<UserActivity>(query, parameters);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching user activity", ex);
            }
        }

        public async Task<IdealActivity> Insert_IdealActivity(IdealActivity activity)
        {
            try
            {
                string query = @"
            INSERT INTO Ideal (userid, organisationid, Ideal_duration,Ideal_Datetime)
            VALUES (@UserId, @OrganisationId, @Ideal_duration,@Ideal_Datetime);
            SELECT CAST(SCOPE_IDENTITY() as int);";

                // Ensure the parameter names match the properties of the 'activity' object
                var parameters = new
                {
                    UserId = activity.UserId,
                    OrganisationId = activity.OrganizationId,
                    Ideal_duration = activity.Ideal_duration,
                    Ideal_DateTime = activity.Ideal_DateTime
                };

                var createdActivityId = await _dapper.ExecuteScalarAsync<int>(query, parameters);
                activity.Id = createdActivityId;
                return activity;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating IdealActivity", ex);
            }
        }

    }
}
