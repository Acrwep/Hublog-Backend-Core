using Dapper;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

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
                string details = JsonConvert.SerializeObject(userBreakModels);
                details = details.Replace("\"null\"", "\"\"");
                details = details.Replace("null", "\"\"");
                details = details.Replace("'", "");

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
                details = JsonConvert.SerializeObject(formattedDetails);

                var parameters = new { details };
                var result = await _dapper.GetAsync<ResultModel>("Exec [SP_BreakEntry] @details", parameters);

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
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@OrganizationId", model.OrganizationId);
                parameters.Add("@AttendanceDate", model.AttendanceDate.ToString("yyyy-MM-dd HH:mm:ss"));
                parameters.Add("@Start_Time", model.Start_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                parameters.Add("@End_Time", model.End_Time?.ToString("yyyy-MM-dd HH:mm:ss"));
                parameters.Add("@Total_Time", null);
                parameters.Add("@Late_Time", null);
                parameters.Add("@Status", model.Status);

                var result = await _dapper.ExecuteAsync("SP_InsertAttendance", parameters, CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting attendance: {ex.Message}");
                return 0;
            }
        }
        #endregion

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
    }
}
