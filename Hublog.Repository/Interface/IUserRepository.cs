using Hublog.Repository.Entities.Model;

namespace Hublog.Repository.Interface
{
    public interface IUserRepository
    {
        Task<int> InsertAttendanceAsync(UserAttendanceModel userAttendanceModel);

        Task<ResultModel> InsertBreak(List<UserBreakModel> userBreakModels);

        Task SaveUserScreenShot(UserScreenShot userScreenShot); 
    }
}
