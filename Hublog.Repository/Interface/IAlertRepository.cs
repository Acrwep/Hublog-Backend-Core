using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Attendance;
using Hublog.Repository.Entities.Model.Break;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hublog.Repository.Interface
{
    public interface IAlertRepository
    {
        Task<int> InsertAlert(Alert Alert);
        Task<List<Alert>> GetAlert(int organizationId, int? teamId, int? userId, DateTime triggeredTime);
        Task<Alert_Rule> InsertAlertRule(Alert_Rule alert_Rule);
        Task<Alert_Rule> UpdateAlertRule(Alert_Rule alert_Rule);
        Task<List<Alert_Rule>> GetAlertRule(int organizationId,string? searchQuery); 
    }
}

