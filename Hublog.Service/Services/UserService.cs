using Hublog.Repository.Entities.DTO;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

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

        public async Task<ResultModel> InsertBreak(List<UserBreakModel> model)
        {
            return await _userRepository.InsertBreak(model);
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
    }
}
