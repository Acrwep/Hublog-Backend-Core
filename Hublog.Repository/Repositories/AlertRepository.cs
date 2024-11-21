using Dapper;
using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Interface;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hublog.Repository.Common;
using System.Globalization;

namespace Hublog.Repository.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        private readonly Dapperr _dapper;

        public AlertRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        public async Task<int> InsertAlert(Alert alert)
        {
            var parameters = new DynamicParameters();
            string startTimeFormatted = alert.TriggeredTime?.ToString("yyyy-MM-dd HH:mm:ss");
            parameters.Add("@UserId", alert.UserId);
            parameters.Add("@Triggered", alert.Triggered); 
            parameters.Add("@TriggeredTime", alert.TriggeredTime?.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine(startTimeFormatted);

            var query = "INSERT INTO Alerts (UserId, Triggered, TriggeredTime) VALUES (@UserId, @Triggered, @TriggeredTime);";

            var result = await _dapper.ExecuteAsync(query, parameters);

            return result; 
        }



        public async Task<List<Alert>> GetAlert(int organizationId, int? userId, DateTime triggeredTime)
        {
            // Define the stored procedure name
            var procedure = "EXEC GetAlerts @OrganizationId, @UserId, @TriggeredTime";
            var parameters = new
            {
                OrganizationId = organizationId,
                UserId = userId,
                TriggeredTime = triggeredTime
            };
            var result = await _dapper.GetAllAsync<Alert>(procedure, parameters);
            return result.ToList();
        }

    }
}

