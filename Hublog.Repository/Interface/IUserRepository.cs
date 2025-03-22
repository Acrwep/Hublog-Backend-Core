using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.UserModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hublog.Repository.Interface
{
    public interface IUserRepository
    {
        Task<int> InsertAttendanceAsync(UserAttendanceModel userAttendanceModel);
        Task<List<UserAttendanceModel>> PunchIn_InsertAttendance(List<UserAttendanceModel> model);
        Task PunchoutInsertAttendance(List<UserAttendanceModel> models);
        Task<ResultModel> InsertBreak(List<UserBreakModel> userBreakModels);

        Task SaveUserScreenShot(UserScreenShot userScreenShot);

        Task<List<UserAttendanceDetailModel>> GetUserAttendanceDetails(int organizationId, int userId, DateTime startDate, DateTime endDate);
        Task<List<UserAttendanceDetailModel>> GetUserPunchInOutDetails(int userId, int organizationId, DateTime startDate, DateTime endDate);
        Task<List<UserAttendanceDetailModel>> UpdateUserAttendanceDetails([FromBody] AttendanceUpdate request);
        Task<List<Users>> GetUsersByTeamId(int teamId); 

        Task<List<Users>> GetUsersByOrganizationId(int organizationId);

        Task<List<BreakMaster>> GetAvailableBreak(int organizationId, DateTime cDate, int userId); 

        Task<BreakMaster> GetBreakMasterById(int id);

        Task<List<UserBreakRecordModel>> GetUserBreakRecordDetails(int userId, DateTime startDate, DateTime endDate);

        Task<List<UsersDTO>> GetAllUser(string loggedInUserEmail, int organizationid, string searchQuery);
        Task<List<UsersDTO>> GetActiveUsers(string loggedInUserEmail, int organizationid, string searchQuery);

        Task<int> InsertUser(Users user);

        Task<int> UpdateUser(Users user); 
        
        Task<int> DeleteUser(int userId);

        Task<List<UserTotalBreakModel>> GetUserTotalBreak(int organizationId, int userId, DateTime startDate, DateTime endDate);
        Task<UserActivity> Insert_Active_Time(UserActivity activity);
        Task<List<UserActivity>> Get_Active_Time(int userid, DateTime startDate, DateTime endDate);
        Task<IdealActivity> Insert_IdealActivity(IdealActivity activity);


        Task<IEnumerable<PunchInUsers>> GetPunchIn_Users(int organizationId, DateTime Date);
       
       
    }
}
