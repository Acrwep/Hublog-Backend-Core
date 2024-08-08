using Hublog.Repository.Entities.Login;
using Hublog.Repository.Entities.Model;

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
    }
}
