﻿using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hublog.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;   
        }

        public async Task InsertAttendance(List<UserAttendanceModel> userAttendanceModels)
        {
            foreach (var model in userAttendanceModels)
            {
                await _userRepository.InsertAttendanceAsync(model);
            }
        }
        public async Task<List<UserAttendanceModel>> PunchIn_InsertAttendance(List<UserAttendanceModel> models)
        { 
              return await _userRepository.PunchIn_InsertAttendance(models);
        }
        public async Task PunchoutInsertAttendance(List<UserAttendanceModel> models)
        {
            await _userRepository.PunchoutInsertAttendance(models);
        }

        public async Task<ResultModel> InsertBreak(List<UserBreakModel> model)
        {
            return await _userRepository.InsertBreak(model);
        }

        public async Task<List<Users>> GetUsersByOrganizationId(int organizationId)
        {
            return await _userRepository.GetUsersByOrganizationId(organizationId);
        }

        public async Task<List<BreakMaster>> GetAvailableBreak(GetModels model)
        {
            return await _userRepository.GetAvailableBreak(model.OrganizationId, model.CDate, model.UserId);
        }

        public async Task<BreakMaster> GetBreakMasterById(int id)
        {
            return await _userRepository.GetBreakMasterById(id);
        }

        public async Task<bool> DeleteUser(int userId)
        {
            var result = await _userRepository.DeleteUser(userId);
            return result > 0;
        }

        #region SaveUserScreenShot
        public async Task SaveUserScreenShot(UserScreenshotDTO userScreenshotDTO)   
        {
            if (userScreenshotDTO.File == null || userScreenshotDTO.File.Length == 0)
            {
                throw new ArgumentException("No file uploaded.");
            }

            var fileName = Path.GetFileName(userScreenshotDTO.File.FileName);
            var fileExtension = Path.GetExtension(fileName);
            var imageName = (userScreenshotDTO.ScreenShotType == "ScreenShots") ? fileName.Replace(fileExtension, "") : Guid.NewGuid().ToString();
            var newFileName = imageName + fileExtension;

            using var ms = new MemoryStream();
            await userScreenshotDTO.File.CopyToAsync(ms);
            var imageData = ms.ToArray();

            if (imageData.Length == 0)
            {
                throw new ArgumentException("Image data is empty.");
            }

            var userScreenShot = new UserScreenShot
            {
                UserId = userScreenshotDTO.UserId,
                OrganizationId = userScreenshotDTO.OrganizationId,
                ScreenShotDate = userScreenshotDTO.ScreenShotDate,
                FileName = newFileName,
                ImageData = imageData
            };

            await _userRepository.SaveUserScreenShot(userScreenShot);
        }
        #endregion

        #region GetUserAttendanceDetails
        public async Task<(List<UserAttendanceDetailModel> Records, AttendanceSummaryModel Summary)> GetUserAttendanceDetails(int organizationId, int userId, DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                DateTime today = DateTime.Today;
                int diff = today.DayOfWeek - DayOfWeek.Monday;
                DateTime startOfWeek = today.AddDays(-diff).Date;
                DateTime endOfWeek = startOfWeek.AddDays(6);

                startDate = startOfWeek;
                endDate = endOfWeek;
            }

            var records = await _userRepository.GetUserAttendanceDetails(organizationId, userId, startDate.Value, endDate.Value);

            var allDates = Enumerable.Range(0, 1 + endDate.Value.Subtract(startDate.Value).Days)
                .Select(offset => startDate.Value.AddDays(offset))
                .ToList();

            int daysPresent = 0;
            int daysAbsent = 0;

            foreach (var date in allDates)
            {
                //if (date.DayOfWeek == DayOfWeek.Sunday) continue;

                var record = records.FirstOrDefault(r => r.AttendanceDate.Date == date.Date);

                if (record != null && TimeSpan.TryParse(record.Total_Time, out var totalTime) && totalTime.TotalMinutes > 0)
                {
                    daysPresent++;
                }
                else
                {
                    daysAbsent++;
                }
            }

            var summary = new AttendanceSummaryModel
            {
                DaysPresent = daysPresent,
                DaysLeave = daysAbsent
            };

            return (records, summary);
        }
        #endregion
        public async Task<List<UserAttendanceDetailModel>> GetUserPunchInOutDetails(int userId, int organizationId, DateTime startDate, DateTime endDate)
        {
            return await _userRepository.GetUserPunchInOutDetails(userId, organizationId, startDate, endDate);
        }
        public async Task<List<UserAttendanceDetailModel>> UpdateUserAttendanceDetails([FromBody] AttendanceUpdate request)
        {
            return await _userRepository.UpdateUserAttendanceDetails(request);
        }
        #region GetUsersByTeamId
        public async Task<object> GetUsersByTeamId(int teamId)  
        {
            var result = await _userRepository.GetUsersByTeamId(teamId);
            var teamData = result.FirstOrDefault();

            if (teamData == null)
            {
                return new
                {
                    Team = new
                    {
                        TeamId = teamId,
                        TeamName = string.Empty,
                        Users = new List<object>()
                    }
                };
            }

            return new
            {
                Team = new
                {
                    TeamId = teamData.TeamId,
                    TeamName = teamData.TeamName,
                    Users = result.Select(u => new
                    {
                        Id = u.Id,
                        First_Name = u.First_Name,
                        Last_Name = u.Last_Name,
                        full_Name = $"{u.First_Name} {u.Last_Name}",
                        Email = u.Email,
                        DOB = u.DOB,
                        DOJ = u.DOJ,
                        Phone = u.Phone,
                        UsersName = u.UsersName,
                        Password = u.Password,
                        Gender = u.Gender,
                        OrganizationId = u.OrganizationId,
                        RoleId = u.RoleId,
                        DesignationId = u.DesignationId,
                        TeamId = u.TeamId,
                        Active = u.Active,
                        EmployeeID = u.EmployeeID,
                        ManagerStatus = u.ManagerStatus,
                    }).ToList()
                }
            };
        }
        #endregion

        #region GetUserBreakRecordDetails
        public async Task<List<UserBreakRecordModel>> GetUserBreakRecordDetails(int userId, DateTime? startDate, DateTime? endDate) 
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                DateTime today = DateTime.Today;
                int diff = today.DayOfWeek - DayOfWeek.Monday;
                startDate = today.AddDays(-diff).Date;
                endDate = startDate.Value.AddDays(6);
            }

            DateTime endDateTime = endDate.Value.Date.AddDays(1);

            return await _userRepository.GetUserBreakRecordDetails(userId, startDate.Value, endDate.Value);
        }
        #endregion

        #region GetAllUser
        public async Task<List<UsersDTO>> GetAllUser(string loggedInUserEmail, int organizationid, string searchQuery)
        {
            var users = await _userRepository.GetAllUser(loggedInUserEmail, organizationid, searchQuery);
            return users;
        }
        #endregion

        #region GetActiveUsers
        public async Task<List<UsersDTO>> GetActiveUsers(string loggedInUserEmail, int organizationid, string searchQuery)
        {
            var users = await _userRepository.GetActiveUsers(loggedInUserEmail, organizationid, searchQuery);
            return users;
        }
        #endregion

        #region  InsertUser
        //public async Task<Users> InsertUser(Users user)
        //{
        //    var userId = await _userRepository.InsertUser(user);

        //    if (userId > 0)
        //    {
        //        user.Id = userId;
        //        return user;
        //    }
        //    else if (userId == -1)
        //    {
        //        return null; 
        //    }
        //    else if (userId == -2)
        //    {
        //        throw new Exception("Organization license count reached. Cannot add more active users.");
        //    }
        //    else
        //    {
        //        throw new Exception("Could not create user");
        //    }
        //}

        public async Task<(Users User, string Error)> InsertUser(Users user)
        {
            var userId = await _userRepository.InsertUser(user);

            if (userId > 0)
            {
                user.Id = userId;
                return (user, null);
            }
            else if (userId == -1)
            {
                return (null, "A user with this email already exists");
            }
            else if (userId == -2)
            {
                return (null, "Organization license count reached. Cannot add more active users.");
            }
            else
            {
                return (null, "Could not create user");
            }
        }
        #endregion



        #region UpdateUser
        public async Task<Users> UpdateUser(Users user)
        {

            // First, fetch the existing user details to check RoleId
            var existingUser = await _userRepository.GetUserById(user.Id);

            if (existingUser == null)
            {
                return null;
            }

            // If the user is an Admin (RoleId = 2), ensure ManagerStatus remains 0
            if (existingUser.RoleId == 2 && user.ManagerStatus == true)
            {
                throw new InvalidOperationException("Admin users cannot be changed to managers");
            }

            if (user.ManagerStatus == true)
            {
                int managerCount = await _userRepository.GetTeamManagerCount(user.TeamId);
                if (managerCount>=2 && existingUser.ManagerStatus==false)
                {
                    throw new InvalidOperationException("This team already has 2 managers. Cannot add another.");
                }
            }
          
            var rowsAffected = await _userRepository.UpdateUser(user);  

            if (rowsAffected > 0)
            {
                return await _userRepository.GetUserById(user.Id);
            }
            else if (rowsAffected == -1)
            {
                throw new InvalidOperationException("The Email is already exists");
            }

            else if (rowsAffected == -2)
            {
                throw new InvalidOperationException("Organization license count reached. Cannot activate more users.");
            }
            else
            {
                return null;
            }
        }
        #endregion

        public async Task<List<UserTotalBreakModel>> GetUserTotalBreak(int organizationId, int userId, DateTime startDate, DateTime endDate)
        {
            return await _userRepository.GetUserTotalBreak(organizationId, userId, startDate, endDate);
        }
        public async Task<UserActivity> Insert_Active_Time(UserActivity activity)
        {
            return await _userRepository.Insert_Active_Time(activity);
        }
        public async Task<List<UserActivity>> Get_Active_Time(int userid, DateTime startDate, DateTime endDate)
        {
            return await _userRepository.Get_Active_Time(userid, startDate, endDate);
        }
        public async Task<IdealActivity> Insert_IdealActivity(IdealActivity activity)
        {
            return await _userRepository.Insert_IdealActivity(activity);
        }

        public async Task<IEnumerable<PunchInUsers>> GetPunchIn_Users(int organizationId, DateTime date)
        {
            var result= await _userRepository.GetPunchIn_Users(organizationId,date);
           return result;
        }
    }
}
