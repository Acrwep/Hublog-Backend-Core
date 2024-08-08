using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface IUserService
    {
        Task InsertAttendance(List<UserAttendanceModel> userAttendanceModels);

        Task<ResultModel> InsertBreak(List<UserBreakModel> model);

        Task SaveUserScreenShot(UserScreenshotDTO userScreenshotDTO);       
    }
}
