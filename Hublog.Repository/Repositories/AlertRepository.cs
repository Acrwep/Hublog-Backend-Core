using Dapper;
using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Interface;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hublog.Repository.Common;

namespace Hublog.Repository.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        private readonly Dapperr _dapper;

        public AlertRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        public async Task<Alert> InsertAlert(Alert alert)
        {
            var query = "INSERT INTO Alerts (UserId, Triggered, TriggeredTime) VALUES (@UserId, @Triggered, @TriggeredTime);";
            var parameters = new { alert.UserId, alert.Triggered, alert.TriggeredTime };
            var result = await _dapper.ExecuteAsync(query, parameters);
            return alert; 
        }

        //public async Task<List<Alert>> GetAlert(int organizationId, int? userId, DateTime triggeredTime)
        //{
        //    var query = @"
        //SELECT 
        //    A.Id,
        //    A.UserId,
        //    A.Triggered,
        //    A.TriggeredTime
        //FROM 
        //    Alerts A
        //INNER JOIN 
        //    Users U ON A.UserId = U.Id
        //INNER JOIN 
        //    Organization O ON U.OrganizationId = O.Id
        //WHERE 
        //    O.Id = @OrganizationId

        //    AND (@UserId IS NULL OR A.UserId = @UserId)
        //    AND (@TriggeredTime IS NULL OR 
        //     (CAST(A.TriggeredTime AS DATE) = CAST(@TriggeredTime AS DATE)))";

        //    // Create parameters
        //    var parameters = new
        //    {
        //        OrganizationId = organizationId,
        //        UserId = userId,
        //        TriggeredTime = triggeredTime
        //    };

        //    // Execute query and return results
        //    var result = await _dapper.GetAllAsync<Alert>(query, parameters);
        //    return result.ToList();
        

        //}
        public async Task<List<Alert>> GetAlert(int organizationId, int? userId, DateTime triggeredTime)
        {
            // Define the stored procedure name
            var procedure = "EXEC GetAlerts @OrganizationId, @UserId, @TriggeredTime";
            // Create parameters for the stored procedure
            var parameters = new
            {
                OrganizationId = organizationId,
                UserId = userId,
                TriggeredTime = triggeredTime
            };

            // Execute the stored procedure and return the results
            var result = await _dapper.GetAllAsync<Alert>(procedure, parameters);
            return result.ToList();
        }

    }
}

