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

        public async Task<List<Alert>> GetAlert(int id, int userId, string triggered, DateTime triggeredTime)
        {
            var query = "SELECT * FROM Alerts WHERE Id = @Id AND UserId = @UserId AND Triggered LIKE @Triggered AND TriggeredTime = @TriggeredTime";
            var parameters = new { Id = id, UserId = userId, Triggered = $"%{triggered}%", TriggeredTime = triggeredTime };
            var result = await _dapper.GetAllAsync<Alert>(query, parameters);
            return result.ToList();

        }
    }
}

