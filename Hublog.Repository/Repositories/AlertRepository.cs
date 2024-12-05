using Dapper;
using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Interface;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hublog.Repository.Common;
using System.Globalization;
using Hublog.Repository.Entities.Model.Break;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Xml.Linq;

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
        public async Task<Alert_Rule> InsertAlertRule(Alert_Rule alert_Rule)
        {
            try
            {
                const string query = @"INSERT INTO Alert_Rule (Name, AlertThreshold,PunchoutThreshold, Status, OrganizationId)
                VALUES (@Name, @AlertThreshold,@PunchoutThreshold, @Status, @OrganizationId);
                SELECT CAST(SCOPE_IDENTITY() as int)";

                var createdBreakmaster = await _dapper.ExecuteAsync(query, alert_Rule);
                alert_Rule.Id = createdBreakmaster;
                return alert_Rule;

            }
            catch (Exception ex)
            {
                throw new Exception("Error creating Breakmaster", ex);
            }
        }
        public async Task<Alert_Rule> UpdateAlertRule(Alert_Rule alert_Rule)
        {
            try
            {
                string query = @" UPDATE alert_Rule 
                                  SET Name = @Name, AlertThreshold = @AlertThreshold,PunchoutThreshold=@PunchoutThreshold, Status = @Status, OrganizationId = @OrganizationId
                                  WHERE Id = @Id"
                ;

                var result = await _dapper.ExecuteAsync(query, alert_Rule);

                if (result > 0)
                {
                    string selectQuery = @"
                                           SELECT Id, Name, AlertThreshold,PunchoutThreshold, Status, OrganizationId
                                           FROM alert_Rule
                                           WHERE Id = @Id";

                    return await _dapper.GetAsync<Alert_Rule>(selectQuery, new { Id = alert_Rule.Id });
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating Breakmaster", ex);
            }
        }
        public async Task<List<Alert_Rule>> GetAlertRule(string searchQuery)
        {
            try
            {
                var query = @"SELECT * FROM Alert_Rule WHERE Name LIKE @SearchQuery";
                var parameter = new { SearchQuery = $"%{searchQuery}%" };
                return await _dapper.GetAllAsync<Alert_Rule>(query, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching breakmaster record", ex);
            }
        }
    }
}

