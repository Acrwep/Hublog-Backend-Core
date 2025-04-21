using System;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;

namespace Hublog.Repository.Repositories
{
    public class ScreenshotRepository : IScreenshotRepository
    {
        private readonly Dapperr _dapper;
        public ScreenshotRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        //public async Task<List<UserScreenShotDTO>> GetUserScreenShots(int userId, int organizationId, DateTime date)
        //{
        //    List<UserScreenShotDTO> userScreenShots = new List<UserScreenShotDTO>();
        //    var query = @"
        //   SELECT uss.[Id], uss.[UserId], uss.[OrganizationId], uss.[ScreenShotDate], 
        //   uss.[FileName], uss.[ImageData], u.[First_Name], u.[Last_Name], u.[Email]
        //   FROM [dbo].[UserScreenShots] uss
        //   JOIN [dbo].[Users] u ON uss.[UserId] = u.[Id]
        //   WHERE uss.[ScreenShotDate] >= @Date 
        //   AND uss.[ScreenShotDate] < DATEADD(DAY, 1, @Date)
        //   AND uss.[UserId] = @UserId
        //   AND uss.[OrganizationId] = @OrganizationId
        //   ORDER BY uss.[ScreenShotDate] ASC"; //Sorting in ascending order

        //     var screenShots = await _dapper.GetAllAsync<UserScreenShot>(query, new
        //    {
        //        UserId = userId,
        //        OrganizationId = organizationId,
        //        Date = date
        //    });

        //    foreach(var screen in screenShots)
        //    {
        //        UserScreenShotDTO userScreen = new UserScreenShotDTO();
        //        userScreen.UserId = screen.UserId;
        //        userScreen.OrganizationId = screen.OrganizationId;
        //        userScreen.ScreenShotDate = screen.ScreenShotDate;
        //        userScreen.FileName = screen.FileName;
        //        userScreen.Base64String = ConvertBase64String(screen.ImageData);
        //        userScreenShots.Add(userScreen);
        //    }
        //    return userScreenShots;
        //}

        public async Task<List<UserScreenShotDTO>> GetUserScreenShots(int userId, int organizationId, DateTime date)
        {
            var query = @"
        SELECT uss.[Id], uss.[UserId], uss.[OrganizationId], uss.[ScreenShotDate], 
               uss.[FileName], uss.[ImageData], u.[First_Name], u.[Last_Name], u.[Email]
        FROM [dbo].[UserScreenShots] uss
        JOIN [dbo].[Users] u ON uss.[UserId] = u.[Id]
        WHERE uss.[ScreenShotDate] >= @Date 
          AND uss.[ScreenShotDate] < DATEADD(DAY, 1, @Date)
          AND uss.[UserId] = @UserId
          AND uss.[OrganizationId] = @OrganizationId
        ORDER BY uss.[ScreenShotDate] ASC";

            var screenShots = await _dapper.GetAllAsync<UserScreenShot>(query, new
            {
                UserId = userId,
                OrganizationId = organizationId,
                Date = date
            });

            return screenShots
                .Select(screen => new UserScreenShotDTO
                {
                    UserId = screen.UserId,
                    OrganizationId = screen.OrganizationId,
                    ScreenShotDate = screen.ScreenShotDate,
                    FileName = screen.FileName,
                    Base64String = ConvertBase64String(screen.ImageData)
                    //Base64String = ConvertToCompressedBase64(screen.ImageData)
                })
                .ToList();
        }


        private string ConvertBase64String(byte[] imageData)
        {
            return imageData != null ? Convert.ToBase64String(imageData) : string.Empty;
        }

        private string ConvertToCompressedBase64(byte[] imageData)
        {
            using var inputStream = new MemoryStream(imageData);
            using var originalImage = System.Drawing.Image.FromStream(inputStream);

            var qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
            var encoderParams = new System.Drawing.Imaging.EncoderParameters(1);
            encoderParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(qualityEncoder, 50L); // 50% quality

            var jpegCodec = System.Drawing.Imaging.ImageCodecInfo
                .GetImageDecoders()
                .First(c => c.FormatID == System.Drawing.Imaging.ImageFormat.Jpeg.Guid);

            using var outputStream = new MemoryStream();
            originalImage.Save(outputStream, jpegCodec, encoderParams);

            return Convert.ToBase64String(outputStream.ToArray());
        }
    }
}
