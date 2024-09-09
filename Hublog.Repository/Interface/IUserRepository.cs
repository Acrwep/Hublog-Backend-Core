using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.UserModels;

namespace Hublog.Repository.Interface
{
    public interface IUserRepository
    {
        Task<int> InsertAttendanceAsync(UserAttendanceModel userAttendanceModel);
           
        Task<ResultModel> InsertBreak(List<UserBreakModel> userBreakModels);

        Task SaveUserScreenShot(UserScreenShot userScreenShot);

        Task<List<UserAttendanceDetailModel>> GetUserAttendanceDetails(int userId, DateTime startDate, DateTime endDate);

        Task<List<Users>> GetUsersByTeamId(int teamId); 

        Task<List<Users>> GetUsersByOrganizationId(int organizationId);

        Task<List<BreakMaster>> GetAvailableBreak(int organizationId, DateTime cDate, int userId); 

        Task<BreakMaster> GetBreakMasterById(int id);

        Task<List<UserBreakRecordModel>> GetUserBreakRecordDetails(int userId, DateTime startDate, DateTime endDate);

        Task<List<UsersDTO>> GetAllUser(string loggedInUserEmail, int organizationid, string searchQuery);

        Task<int> InsertUser(Users user);

        Task<int> UpdateUser(Users user); 
        
        Task<int> DeleteUser(int userId);

        Task TrackApplicationUsage(int userId, string applicationName, string totalUsage, string details, DateTime usageDate);

        Task<List<GetApplicationUsage>> GetUsersApplicationUsages(int organizationId, int userId, DateTime startDate, DateTime endDate); 
    }
}
