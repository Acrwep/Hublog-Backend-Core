using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Login;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.UserModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hublog.Service.Interface
{
    public interface IUserService
    {
        Task InsertAttendance(List<UserAttendanceModel> userAttendanceModels);
        Task PunchIn_InsertAttendance(List<UserAttendanceModel> model);
        Task PunchoutInsertAttendance(List<UserAttendanceModel> models);
        Task<ResultModel> InsertBreak(List<UserBreakModel> model);

        Task SaveUserScreenShot(UserScreenshotDTO userScreenshotDTO);

        Task<(List<UserAttendanceDetailModel> Records, AttendanceSummaryModel Summary)> GetUserAttendanceDetails(int organizationId, int userId, DateTime? startDate, DateTime? endDate);
        Task<List<UserAttendanceDetailModel>> GetUserPunchInOutDetails(int userId, int organizationId ,DateTime startDate, DateTime endDate);
        Task<List<UserAttendanceDetailModel>> UpdateUserAttendanceDetails([FromBody] AttendanceUpdate request);
        Task<object> GetUsersByTeamId(int teamId);  

        Task<List<Users>> GetUsersByOrganizationId(int organizationId);

        Task<List<BreakMaster>> GetAvailableBreak(GetModels model); 

        Task<BreakMaster> GetBreakMasterById(int id);

        Task<List<UserBreakRecordModel>> GetUserBreakRecordDetails(int userId, DateTime? startDate, DateTime? endDate);

        Task<List<UsersDTO>> GetAllUser(string loggedInUserEmail, int organizationid, string searchQuery);

        Task<Users> InsertUser(Users user);

        Task<Users> UpdateUser(Users user); 

        Task<bool> DeleteUser(int userId);

        Task<List<UserTotalBreakModel>> GetUserTotalBreak(int organizationId, int userId, DateTime startDate, DateTime endDate);

        Task<UserActivity> Insert_Active_Time(UserActivity activity);
        Task<List<UserActivity>> Get_Active_Time(int userid, DateTime startDate, DateTime endDate);

        Task<IdealActivity> Insert_IdealActivity(IdealActivity activity);
    }
}
